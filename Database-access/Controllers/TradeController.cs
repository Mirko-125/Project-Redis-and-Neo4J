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
        public async Task<IActionResult> AddTrade(Trade trade, int player1ID,int player2ID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Trade {
                            isFinalized: $isFinalized,
                            receiverGold: $receiverGold,
                            requesterGold: $requesterGold,
                            startedAt: $startedAt,
                            endedAt: $endedAt
                        })
                        WITH n
                        MATCH (n1:Player) WHERE id(n1)=$playeri1
                        MATCH (n2:Player) WHERE id(n2)=$playeri2
                        CREATE (n)-[:RECEIVER]->(n1)
                        CREATE (n)-[:REQUESTER]->(n2)"
                    ;

                    var parameters = new
                    {
                        isFinalized=trade.IsFinalized,
                        receiverGold=trade.ReceiverGold,
                        requesterGold=trade.RequesterGold,
                        startedAt=trade.StartedAt,
                        endedAt=trade.EndedAt,
                        playeri1=player1ID,
                        playeri2=player2ID
                    };
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
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
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
                        var query = "MATCH (n1:Player)-[:RECEIVER]->(n:Trade)<-[:REQUESTER]-(n2:Player) WHERE id(n1)=$playeri1 and id(n2)=$playeri2 RETURN n";
                        
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteTrade")]
        public async Task<IActionResult> DeleteTrade(int tradeID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Trade) WHERE id(n)=$tradeid DETACH DELETE n";
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