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
        public async Task<IActionResult> AddMonsterBattle(MonsterBattle mb, int monsterId,int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:MonsterBattle {
                            startedAt: $startedAt,
                            endedAt: $endedAt,
                            isFinalized: $isFinalized
                        }) 
                        WITH n
                        MATCH (n1:Monster) WHERE id(n1)=$monsteri
                        MATCH (n2:Player) WHERE id(n2)=$playeri
                        CREATE (n1)<-[:ATTACKED_MONSTER]-(n)-[:ATTACKING_PLAYER]->(n2)";

                    var parameters = new
                    {
                        startedAt=mb.StartedAt,
                        endedAt=mb.EndedAt,
                        isFinalized=mb.IsFinalized,
                        playeri=playerId,
                        monsteri=monsterId
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
        [HttpGet("GetMonsterBattle")]
        public async Task<IActionResult> GetMonsterBattle(int monsterBattleId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:MonsterBattle) WHERE id(n)=$idn RETURN n";
                        var parameters = new { idn = monsterBattleId };
                        var cursor = await tx.RunAsync(query,parameters);

                         var n=await cursor.SingleAsync();
                        var node = n["n"].As<INode>();
                        return node;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
        public async Task<IActionResult> GetMonsterBattlesOfPlayer(int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:MonsterBattle)-[:ATTACKING_PLAYER]->(n1:Player) WHERE id(n1)=$playeri RETURN n";
                        var parameters = new { playeri = playerId };
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
        [HttpPut("UpdateMonsterBattle")]
        public async Task<IActionResult> UpdateMonsterBattle(int mbId, string newEndedAt ,bool newIsFinalized)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:MonsterBattle) WHERE ID(n)=$monsterBattleId
                                SET n.endedAt= $endedAt
                                SET n.isFinalized= $isFinalized
                                RETURN n";
                    var parameters = new { monsterBattleId = mbId,
                                        endedAt = newEndedAt,
                                        isFinalized= newIsFinalized };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteMonsterBattle")]
        public async Task<IActionResult> RemoveMonsterBattle(int mbId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
                    var query = @"MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(n2:Player) WHERE id(n)=$mb DETACH DELETE n";
                    var parameters = new { mb = mbId};
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        #region Loot
        [HttpPost("AddLoot")]
        public async Task<IActionResult> AddLoot(int monsterBattleId,int itemId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        MATCH (n1:Item) WHERE id(n1)=$itId
                        MATCH (n2:MonsterBattle) WHERE id(n2)=$monsteri
                        CREATE (n2)-[:LOOT]->(n1)";

                    var parameters = new
                    {
                        itId=itemId,
                        monsteri=monsterBattleId
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
        [HttpGet("GetLoot")]
        public async Task<IActionResult> GetLoot(int monsterBattleId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:MonsterBattle)-[:LOOT]->(n1:Item) WHERE id(n)=$id RETURN n1";
                        var parameters = new{id=monsterBattleId};
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n1"].As<INode>();
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
        
        #endregion Loot
        /*
       [HttpDelete("DeleteMonsterBattles")]
        public async Task<IActionResult> RemoveMonsterBattle(int monsterId,int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(n2:Player) WHERE id(n2)=$playeri and id(n1)=$monsteri DETACH DELETE n";
                    var parameters = new { monsteri = monsterId,
                    playeri=playerID };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/
    }
}