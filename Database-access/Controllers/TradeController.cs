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

        [HttpGet]
        public async Task<IActionResult> GetAllNodes()
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

         #region AddTrade

        [HttpPost]
        public async Task<IActionResult> AddTrade(Trade trade)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Trade {
                            isComplete: $isComplete,
                            receiverGold: $receiverGold,
                            requesterGold: $requesterGold,
                            startedAt: $startedAt,
                            endedAt: $endedAt
                        })";

                    var parameters = new
                    {
                        isComplete=trade.IsComplete,
                        receiverGold=trade.ReceiverGold,
                        requesterGold=trade.RequesterGold,
                        startedAt=trade.StartedAt,
                        endedAt=trade.EndedAt
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
        #endregion

        
   
        
    }
}