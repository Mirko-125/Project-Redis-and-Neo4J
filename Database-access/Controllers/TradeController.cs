using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly IDriver _driver;

        public TradeController(IDriver driver)
        {
            _driver = driver;
        }


        [HttpPost("AddTrade")]
        public async Task<IActionResult> AddTrade(TradeDto trade)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new
                    {
                        isFinalized=trade.IsFinalized,  
                        receiverGold=trade.ReceiverGold, 
                        requesterGold=trade.RequesterGold,
                        startedAt=trade.StartedAt,
                        endedAt=trade.EndedAt,
                        receiverID=trade.ReceiverID,
                        requesterID=trade.RequesterID,
                        receiverItemNames=trade.ReceiverItemNames,
                        requesterItemNames=trade.RequesterItemNames
                    };

                    if(trade.ReceiverItemNames.Length>0){
                        var playerReceverQuery=@"
                            MATCH(playerReceiver:Player)-[:OWNS]->(inventoryReceiver: Inventory)
                                WHERE ID(playerReceiver)=$receiverID
                                
                            MATCH (receiverItem :Item)
                                WHERE receiverItem.name
                                IN $receiverItemNames
                                AND (inventoryReceiver)-[:HAS]->(receiverItem)
                                
                            return receiverItem"
                        ;

                        var playerReceiverItemResult=await session.RunAsync(playerReceverQuery, parameters);
                        var receiverItems=await playerReceiverItemResult.ToListAsync();

                        Console.WriteLine("Added item");

                        if(receiverItems.Count!=trade.ReceiverItemNames.Length)
                            throw new Exception("The playerReceiver doesn't own the items");
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

                        Console.WriteLine("Added item");

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
                        
                    FOREACH (item IN receiverItemsList |
                            CREATE (n)-[:RECEIVER_ITEM]->(item)
                        )

                        FOREACH (item IN requesterItemsList |
                            CREATE (n)-[:REQUESTER_ITEM]->(item)
                        )
                        
                        CREATE (n)-[:RECEIVER]->(playerReceiver_)
                        CREATE (n)-[:REQUESTER]->(playerRequester_)
                        
                        return n"
                    ;

                    var result=await session.RunAsync(query, parameters);
                    return Ok();
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
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Trade) RETURN n";
                        var cursor = await tx.RunAsync(query);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
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
                            MATCH (n:Trade)-[:REQUESTER]->(player:Player)
                                WHERE ID(player) = $playerid
                            RETURN n AS trade, 'REQUESTER' AS role

                            UNION

                            MATCH (n:Trade)-[:RECEIVER]->(player:Player)
                                WHERE ID(player) = $playerid
                            RETURN n AS trade, 'RECEIVER' AS role"
                        ;
                                                  
                        var parameters = new { playerid = playerID };
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["trade"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTradesBetweenTwoPlayers")]
        public async Task<IActionResult> GetTradesBetweenTwoPlayers(int player1ID, int player2ID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n1:Player)<-[:RECEIVER]-(n:Trade)-[:REQUESTER]->(n2:Player) WHERE id(n1)=$playeri1 and id(n2)=$playeri2 RETURN n";
                        
                        var parameters = new { playeri1 = player1ID,
                        playeri2=player2ID};
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateTrade")]
        public async Task<IActionResult> UpdateTrade(int tradeid, string endedAt, bool isFinalized)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Trade) WHERE ID(n)=$tradeID
                                SET n.endedAt= $endedAt
                                SET n.isFinalized= $isFinalized
                                RETURN n";
                    var parameters = new { tradeID = tradeid,
                                           endedAt = endedAt,
                                           isFinalized= isFinalized
                                         };
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