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
    public class MonsterBattleController : ControllerBase
    {
        private readonly IDriver _driver;

        public MonsterBattleController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("AddMonsterBattle")]
        public async Task<IActionResult> AddMonsterBattle(MonsterBattle mb, string monster,int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:MonsterBattle {
                            status: $status,
                            startedAt: $startedAt,
                            endedAt: $endedAt,
                            finalized: $finalized
                        })
                        WITH n
                        MATCH (n1:Monster {name: $monsteri})
                        MATCH (n2:Player) WHERE id(n2)=$playeri
                        CREATE (n1)-[:ATTACKED_IN]->(n)<-[:ATTACKING_IN]-(n2)
                         ";

                    var parameters = new
                    {
                        status= mb.Status,
                        startedAt=mb.StartedAt,
                        endedAt=mb.EndedAt,
                        finalized=mb.Finalized,
                        playeri=playerID,
                        monsteri=monster
                    };
                    var result=await session.RunAsync(query, parameters);
                   /* var idn=await result.SingleAsync();
                    var nodeId = idn["nodeId"].As<long>();
                    var query1 = @"
                        MATCH (n1:Monster {name: $monsteri})
                        MATCH (n2:Player) WHERE id(n2)=$playeri
                        MATCH (n:MonsterBattle) WHERE id(n)=$idn
                        CREATE (n1)-[:ATTACKED_IN]->(n)<-[:ATTACKING_IN]-(n2)
                       ";
                    var parameters1 = new
                    {
                       playeri=playerID,
                       monsteri=monster,
                       idn=nodeId
                    };
                    await session.RunAsync(query1, parameters1);*/
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllMonsterBattles")]
        public async Task<IActionResult> GetAllMonsterBattles()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:MonsterBattle) RETURN n";
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
        [HttpGet("GetMonsterBattlesOfPlayer")]
        public async Task<IActionResult> GetMonsterBattlesOfPlayer(int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:MonsterBattle)<-[:ATTACKING_IN]-(n1:Player) WHERE id(n1)=$playeri RETURN n";
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
       [HttpDelete("DeleteMonsterBattle")]
        public async Task<IActionResult> RemoveMonsterBattle(string monster,int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n1:Monster {name: $monsteri})-[:ATTACKED_IN]->(n:MonsterBattle)<-[:ATTACKING_IN]-(n2:Player) WHERE id(n2)=$playeri DETACH DELETE n";
                    var parameters = new { monsteri = monster,
                    playeri=playerID };
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