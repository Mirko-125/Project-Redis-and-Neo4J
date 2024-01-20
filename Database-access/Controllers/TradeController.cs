using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using ServiceStack.Redis;
using Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using ServiceStack.Text;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly IDriver _driver;
        private readonly RedisCache cache;
        private readonly string singularKey = "trade";
        //private readonly string pluralKey = "trades";

        public TradeController(IDriver driver, RedisCache redisCache)
        {
            _driver = driver;
            cache = redisCache;
        }

        [HttpPost("AddTrade")] 
        public async Task<IActionResult> AddTrade(TradeCreateDto trade)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new
                    {
                        isFinalized=false,  
                        receiverGold=trade.ReceiverGold, 
                        requesterGold=trade.RequesterGold,
                        startedAt=DateTime.Now,
                        endedAt="--",
                        receiverID=trade.ReceiverID,
                        requesterID=trade.RequesterID,
                        receiverItemNames=trade.ReceiverItemNames,
                        requesterItemNames=trade.RequesterItemNames
                    };

                    if(trade.RequesterGold>0 ){
                        var goldCheckQuery = @"MATCH (playerRequester:Player) WHERE ID(playerRequester)=$requesterID
                                                RETURN playerRequester.gold 
                                              ";

                        var goldCheckReqResult = await session.RunAsync(goldCheckQuery, parameters);
                        var goldReqCheckList = await goldCheckReqResult.ToListAsync();

                        if(goldReqCheckList.Count==0)
                        {
                            throw new Exception("The playerRequester doesn't own enought gold!");
                        }   
                    }

                    if(trade.RequesterItemNames.Length>0){
                        var playerRequesterQuery=@"
                            MATCH(playerRequester:Player)-[:OWNS]->(inventoryRequester: Inventory)
                                WHERE ID(playerRequester)=$requesterID
                                
                            MATCH (requesterItem :Item)
                                WHERE requesterItem.name
                                IN $requesterItemNames
                                AND (inventoryRequester)-[:HAS]->(requesterItem)
                                
                            return requesterItem"
                        ;

                        var playerRequesterItemResult=await session.RunAsync(playerRequesterQuery, parameters);
                        var requesterItems=await playerRequesterItemResult.ToListAsync();

                        if(requesterItems.Count!=trade.RequesterItemNames.Length)
                            throw new Exception("The playerRequester doesn't own the items");
                    }
                
                    var query=@"
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
                        
                        CREATE (n:Trade {
                                isFinalized: $isFinalized,
                                receiverGold: $receiverGold,
                                requesterGold: $requesterGold,
                                startedAt: $startedAt,
                                endedAt: $endedAt
                        })
                            
                        WITH n, playerReceiver_, playerRequester_, receiverItemsList, requesterItemsList
                             
                        CREATE (n)-[:RECEIVER]->(playerReceiver_)
                        CREATE (n)-[:REQUESTER]->(playerRequester_)

                        FOREACH (item IN receiverItemsList |
                            MERGE (n)-[:RECEIVER_ITEM]->(item)
                        )

                        FOREACH (item IN requesterItemsList |
                            MERGE (n)-[:REQUESTER_ITEM]->(item)
                        )

                        return Id(n) as tradeID
                        "
                        ; 

                        var returnQuery = @"MATCH (receiver:Player)<-[relReceiver:RECEIVER]-(trade:Trade)-[relRequester:REQUESTER]->(requester:Player)
                                        ,(recItem:Item)<-[relReceiverItem:RECEIVER_ITEM]-(trade)-[relRequesterItem:REQUESTER_ITEM]->(reqItem:Item)
                                        WHERE Id(trade) = $ID
                                            OPTIONAL MATCH (recItem)-[r:HAS]->(a:Attributes)
                                            OPTIONAL MATCH (reqItem)-[r:HAS]->(a:Attributes)
                                        RETURN 
                                        trade as tradeInfo, 
                                        requester, 
                                        receiver, 
                                        COLLECT({ item: recItem,
                                                    attributes: CASE WHEN recItem:Gear THEN a ELSE NULL END
                                                }) AS itemsRec,

                                        COLLECT({ item: reqItem,
                                                    attributes: CASE WHEN reqItem:Gear THEN a ELSE NULL END
                                                }) AS itemsReq";

                        var idCursor = await session.RunAsync(query, parameters);
                        var idRecord = await idCursor.SingleAsync();
                        var tradeID = idRecord ["tradeID"].As<int>();
                        var cursor = await session.RunAsync(returnQuery, new {ID = tradeID});
                        var record = await cursor.SingleAsync();   
                        var tradeNode = record["tradeInfo"].As<INode>();
                    
                        var playerRec = record["receiver"].As<INode>();
                        var playerReq = record["requester"].As<INode>();
                        var recItem = record["itemsRec"].As<List<Dictionary<string, INode>>>();
                        var reqItem = record["itemsReq"].As<List<Dictionary<string, INode>>>();
                        Trade createTrade = new(tradeNode, playerRec, playerReq, recItem, reqItem);
                        string key = singularKey + tradeID;
                        await cache.SetDataAsync(key, createTrade, 1000);
                        return Ok(createTrade);
    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllTrades")]
        public async Task<IActionResult> GetAllTrades()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    string key = "trades";
                    var keyExists = await cache.CheckKeyAsync(key);
                    if (keyExists)
                    {
                        var t = await cache.GetDataAsync<List<Trade>>(key);
                        return Ok(t);          
                    }
                        var query = @"MATCH (receiver:Player)<-[relReceiver:RECEIVER]-(trade:Trade)-[relRequester:REQUESTER]->(requester:Player),
                                            (recItem:Item)<-[relReceiverItem:RECEIVER_ITEM]-(trade)-[relRequesterItem:REQUESTER_ITEM]->(reqItem:Item)
                                      OPTIONAL MATCH (recItem)-[r:HAS]->(a:Attributes)
                                      OPTIONAL MATCH (reqItem)-[r:HAS]->(a:Attributes)
                                      RETURN 
                                      trade as tradeInfo, 
                                      requester, 
                                      receiver, 
                                      COLLECT({ item: recItem,
                                                attributes: CASE WHEN recItem:Gear THEN a ELSE NULL END
                                             }) AS itemsRec,

                                      COLLECT({ item: reqItem,
                                                attributes: CASE WHEN reqItem:Gear THEN a ELSE NULL END
                                             }) AS itemsReq";
                        var cursor = await session.RunAsync(query);
                        var resultList = new List<Trade>();

                        await cursor.ForEachAsync(record =>
                        {
                            var tradeNode = record["tradeInfo"].As<INode>();
                            var playerRec = record["receiver"].As<INode>();
                            var playerReq = record["requester"].As<INode>();
                            var recItem = record["itemsRec"].As<List<Dictionary<string, INode>>>();
                            var reqItem = record["itemsReq"].As<List<Dictionary<string, INode>>>();
                            Trade trade = new(tradeNode, playerRec, playerReq, recItem, reqItem);
                            resultList.Add(trade);
                        });

                        await cache.SetDataAsync(key, resultList, 1000);
                        return Ok(resultList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPlayerTrades")]
        public async Task<IActionResult> GetPlayerTrades(int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"
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
                                COLLECT(DISTINCT { item: recItem, attributes: recAttr }) AS itemsRec,
                                COLLECT(DISTINCT { item: reqItem, attributes: reqAttr }) AS itemsReq";
                                                  
                        var parameters = new { playerid = playerID };
                        var cursor = await tx.RunAsync(query,parameters);
                        var resultList = new List<Trade>();

                        await cursor.ForEachAsync(record =>
                        {
                            var tradeNode = record["tradeInfo"].As<INode>();
                            var playerRec = record["receiver"].As<INode>();
                            var playerReq = record["requester"].As<INode>();
                            var recItem = record["itemsRec"].As<List<Dictionary<string, INode>>>();
                            var reqItem = record["itemsReq"].As<List<Dictionary<string, INode>>>();
                            Trade trade = new(tradeNode, playerRec, playerReq, recItem, reqItem);
                            resultList.Add(trade);
                        });
                    
                        return Ok(resultList);
                    });
                    
                    return Ok(result);
                
            }
        }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("FinalizeTrade")]
        public async Task<IActionResult> FinalizeTrade(TradeUpdateDto trade)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Trade) WHERE ID(n)=$tradeID
                                SET n.receiverGold = $receiverGold
                                SET n.requesterGold = $requesterGold
                                SET n.isFinalized = $isFinalized
                                SET n.endedAt=$endedAt
                                RETURN n";

                    var parameters = new
                    {
                        tradeID = trade.TradeID,
                        receiverGold = trade.ReceiverGold,
                        requesterGold = trade.RequesterGold,
                        isFinalized = true, 
                        endedAt = DateTime.Now
                    };
                    await cache.DeleteAsync(singularKey + trade.TradeID);
                    await session.RunAsync(query, parameters);

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteTrade")]
        public async Task<IActionResult> DeleteTrade(int tradeID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = "MATCH (n:Trade) WHERE id(n)=$tradeid DETACH DELETE n";
                    var parameters = new { tradeid = tradeID };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }   
    }
}