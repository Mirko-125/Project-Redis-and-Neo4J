using Cache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class TradeService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "Trade";
        public readonly string _pluralKey = "trades";
        public readonly string _key = "trade";

        private static Trade BuildTrade(IRecord record)
        {
            var tradeNode = record["tradeInfo"].As<INode>();
            var playerRec = record["receiver"].As<INode>();
            var playerReq = record["requester"].As<INode>();
            var recItem = record["itemsRec"].As<List<Dictionary<string, INode>>>();
            var reqItem = record["itemsReq"].As<List<Dictionary<string, INode>>>();
            return new Trade(tradeNode, playerRec, playerReq, recItem, reqItem);
        }

        public TradeService(IDriver driver, RedisCache cache)
        {
            _driver = driver;
            _cache = cache;
        }

        public async Task<Trade> CreateAsync(TradeCreateDto tradeDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                isFinalized=false,  
                receiverGold=tradeDto.ReceiverGold, 
                requesterGold=tradeDto.RequesterGold,
                startedAt=DateTime.Now,
                endedAt="--",
                receiverID=tradeDto.ReceiverID,
                requesterID=tradeDto.RequesterID,
                receiverItemNames=tradeDto.ReceiverItemNames,
                requesterItemNames=tradeDto.RequesterItemNames
            };

            if(tradeDto.RequesterItemNames.Length>0){
                var playerRequesterQuery=$@"
                    MATCH(playerRequester:Player)-[:OWNS]->(inventoryRequester: Inventory)
                        WHERE ID(playerRequester)=$requesterID
                                
                    MATCH (requesterItem :Item)
                        WHERE requesterItem.name
                        IN $requesterItemNames
                        AND (inventoryRequester)-[:CONTAINS]->(requesterItem)
                                
                    return requesterItem
                    ";

            var playerRequesterItemResult=await session.RunAsync(playerRequesterQuery, parameters);
            var requesterItems=await playerRequesterItemResult.ToListAsync();

            if(requesterItems.Count!=tradeDto.RequesterItemNames.Length)
                throw new Exception("The playerRequester doesn't own the items");
            }


            var query = $@"
                MATCH (playerReceiver_:Player)
                    WHERE id(playerReceiver_)=$receiverID
                            
                MATCH (receiverItem: Item)
                    WHERE receiverItem.name IN $receiverItemNames
                WITH playerReceiver_, collect(receiverItem) as receiverItemsList
                        
                MATCH (playerRequester_:Player)
                    WHERE id(playerRequester_)=$requesterID
                            
                MATCH (requesterItem: Item)
                    WHERE requesterItem.name IN $requesterItemNames
                WITH playerReceiver_, receiverItemsList, playerRequester_, collect(requesterItem) as requesterItemsList
                        
                CREATE (n:{type} {{
                    isFinalized: $isFinalized,
                    receiverGold: $receiverGold,
                    requesterGold: $requesterGold,
                    startedAt: $startedAt,
                    endedAt: $endedAt
                }})
                            
                WITH n, playerReceiver_, playerRequester_, receiverItemsList, requesterItemsList
                             
                CREATE (n)-[:RECEIVER]->(playerReceiver_)
                CREATE (n)-[:REQUESTER]->(playerRequester_)

                FOREACH (item IN receiverItemsList |
                    CREATE (n)-[:RECEIVER_ITEM]->(item)
                )

                FOREACH (item IN requesterItemsList |
                    CREATE (n)-[:REQUESTER_ITEM]->(item)
                )

                return Id(n) as tradeID";

            var returnQuery = $@"
                MATCH (receiver:Player)<-[relReceiver:RECEIVER]-(trade:Trade)-[relRequester:REQUESTER]->(requester:Player),
                       (recItem:Item)<-[relReceiverItem:RECEIVER_ITEM]-(trade)-[relRequesterItem:REQUESTER_ITEM]->(reqItem:Item)
                WHERE Id(trade) = $ID
                OPTIONAL MATCH (recItem)-[r:HAS]->(recA:Attributes)
                OPTIONAL MATCH (reqItem)-[r:HAS]->(reqA:Attributes)
                RETURN 
                trade as tradeInfo, 
                requester, 
                receiver, 
                COLLECT({{ item: recItem,
                           attributes: CASE WHEN recItem:Gear THEN recA ELSE NULL END
                        }}) AS itemsRec,

                COLLECT({{ item: reqItem,
                           attributes: CASE WHEN reqItem:Gear THEN reqA ELSE NULL END
                        }}) AS itemsReq";

            Console.WriteLine(query);
            var idCursor = await session.RunAsync(query, parameters);
            var idRecord = await idCursor.SingleAsync();
            var tradeID = idRecord ["tradeID"].As<int>();
            var cursor = await session.RunAsync(returnQuery, new {ID = tradeID});
            var record = await cursor.SingleAsync();  
            Trade trade = BuildTrade(record);
            await _cache.SetDataAsync(_key+tradeID, trade, 1000);
            return trade;
        }

        public async Task<List<Trade>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var keyExists = await _cache.CheckKeyAsync(_pluralKey);
            if (keyExists)
            {
                var cachedTrades = await _cache.GetDataAsync<List<Trade>>(_pluralKey);
                return cachedTrades;          
            }

            string query = $@"
                MATCH (receiver:Player)<-[relReceiver:RECEIVER]-(trade:Trade)-[relRequester:REQUESTER]->(requester:Player),
                      (recItem:Item)<-[relReceiverItem:RECEIVER_ITEM]-(trade)-[relRequesterItem:REQUESTER_ITEM]->(reqItem:Item)
                OPTIONAL MATCH (recItem)-[r:HAS]->(a:Attributes)
                OPTIONAL MATCH (reqItem)-[r:HAS]->(a:Attributes)
                RETURN 
                trade as tradeInfo, 
                id(trade) as tradeID,
                requester, 
                receiver, 
                COLLECT({{ item: recItem,
                        attributes: CASE WHEN recItem:Gear THEN a ELSE NULL END
                        }}) AS itemsRec,

                COLLECT({{ item: reqItem,
                        attributes: CASE WHEN reqItem:Gear THEN a ELSE NULL END
                        }}) AS itemsReq
                ";
     
            var cursor = await session.RunAsync(query);
            var trades = new List<Trade>();
            Console.WriteLine(query);
            await cursor.ForEachAsync(record =>
            {
                trades.Add(BuildTrade(record));
            });

            foreach (Trade trade in trades)
            {
                await _cache.SetDataAsync(_key, trade, 100);
            }
            await _cache.SetDataAsync(_pluralKey, trades, 100);

            return trades;
        }

        
        
        public async Task<List<Trade>> GetPlayerTradesAsync(int playerid)
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH (receiver:Player)<-[relReceiver:RECEIVER]-(trade:Trade)-[relRequester:REQUESTER]->(requester:Player),
                      (recItem:Item)<-[relReceiverItem:RECEIVER_ITEM]-(trade)-[relRequesterItem:REQUESTER_ITEM]->(reqItem:Item)
                WHERE ID(receiver) = $playerid OR ID(requester) = $playerid
                OPTIONAL MATCH (recItem)-[r:HAS]->(recAttr:Attributes)
                WHERE recItem:Gear
                OPTIONAL MATCH (reqItem)-[r:HAS]->(reqAttr:Attributes)
                WHERE reqItem:Gear
                RETURN 
                trade as tradeInfo, 
                requester, 
                receiver, 
                COLLECT(DISTINCT {{ item: recItem, attributes: recAttr }}) AS itemsRec,
                COLLECT(DISTINCT {{ item: reqItem, attributes: reqAttr }}) AS itemsReq ";
           
            var cursor = await session.RunAsync(query, new {playerid = playerid});
            var trades = new List<Trade>();
            await cursor.ForEachAsync(record =>
            {
               trades.Add(BuildTrade(record));
            });
                
            return trades;      
        }

        public async Task<IResultCursor> FinalizeAsync(TradeUpdateDto tradeDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                tradeID = tradeDto.TradeID,
                receiverGold = tradeDto.ReceiverGold,
                requesterGold = tradeDto.RequesterGold,
                isFinalized = true, 
                endedAt = DateTime.Now
            };

            string query = @$"
                MATCH (n:{type}) WHERE ID(n)=$tradeID
                    SET n.receiverGold = $receiverGold
                    SET n.requesterGold = $requesterGold
                    SET n.isFinalized = $isFinalized
                    SET n.endedAt=$endedAt
                RETURN n";
            await _cache.DeleteAsync(_key + tradeDto.TradeID);
            return await session.RunAsync(query, parameters);
        }

        public async Task<IResultCursor> DeleteAsync(int tradeID)
        {
            var session = _driver.AsyncSession();
            var query = @$"MATCH (n:{type}) WHERE id(n)=$tradeID DETACH DELETE n";
            var parameters = new {tradeID};
            return await session.RunAsync(query, parameters);
        }
    }     
}