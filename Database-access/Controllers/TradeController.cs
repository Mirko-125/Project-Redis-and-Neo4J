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

        [HttpGet("GetTradeOfPlayerReceiver")]
        public async Task<IActionResult> GetTradeOfPlayerReceiver(int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Trade)<-[:RECEIVER]-(n1:Player) WHERE id(n1)=$playeri RETURN n";
                                     
                        
                        var parameters = new { playeri = playerID };
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

        [HttpGet("GetTradeOfPlayerRequester")]
        public async Task<IActionResult> GetTradeOfPlayerRequester(int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Trade)<-[:REQUESTER]-(n1:Player) WHERE id(n1)=$playeri RETURN n";
                                     
                        
                        var parameters = new { playeri = playerID };
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

        [HttpGet("GetTradesBetweenTwoPlayers")]
        public async Task<IActionResult> GetTradesBetweenTwoPlayers(int player1ID,int player2ID)
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

        [HttpGet("GetTradesBetweenPlayerAndMarketplace")]
        public async Task<IActionResult> GetTradesBetweenPlayerAndMarketplace(int playerID,int marketplaceID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n1:Player)-[:RECEIVER]->(n:Trade)<-[:REQUESTER]-(n2:Marketplace) WHERE id(n1)=$player and id(n2)=$marketplace RETURN n";
                        
                        var parameters = new { player = playerID,
                        marketplace=marketplaceID};
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

        [HttpGet("GetTradesOfPlayerAndItem")]
        public async Task<IActionResult> GetTradesOfPlayerAndItem(int playerID,int itemID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n1:Player)-[:RECEIVER_ITEMS]->(n:Trade)<-[:REQUESTER_ITEMS]-(n2:Item) WHERE id(n1)=$player and id(n2)=$item RETURN n";
                        
                        var parameters = new { player = playerID,
                        item=itemID};
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
        

        [HttpPost("AddTradeOfPlayers")]
        public async Task<IActionResult> AddTradeOfPlayers(Trade trade, int player1ID,int player2ID)
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
                        CREATE (n1)-[:RECEIVER]->(n)<-[:REQUESTER]-(n2) ";

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
        
   
        [HttpPost("AddTradeOfPlayerAndMarketplace")]
        public async Task<IActionResult> AddTradeOfPlayerAndMarketplace(Trade trade, int playerID,int marketplaceID)
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
                        MATCH (n1:Player) WHERE id(n1)=$player
                        MATCH (n2:Marketplace) WHERE id(n2)=$marketplace
                        CREATE (n1)-[:RECEIVER]->(n)<-[:REQUESTER]-(n2) ";

                    var parameters = new
                    {
                        isFinalized=trade.IsFinalized,
                        receiverGold=trade.ReceiverGold,
                        requesterGold=trade.RequesterGold,
                        startedAt=trade.StartedAt,
                        endedAt=trade.EndedAt,
                        player=playerID,
                        marketplace=marketplaceID
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

        [HttpPost("AddTradeOfPlayerAndItem")]
        
        public async Task<IActionResult> AddTradeOfPlayerAndItem(Trade trade, int playerID, int itemID)
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
                        MATCH (n1:Player) WHERE id(n1)=$player
                        MATCH (n2:Item) WHERE id(n2)=$item
                        CREATE (n1)-[:RECEIVER_ITEMS]->(n)<-[:REQUESTER_ITEMS]-(n2) ";

                    var parameters = new
                    {
                        isFinalized=trade.IsFinalized,
                        receiverGold=trade.ReceiverGold,
                        requesterGold=trade.RequesterGold,
                        startedAt=trade.StartedAt,
                        endedAt=trade.EndedAt,
                        player=playerID,
                        item=itemID
                       
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