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
        public async Task<IActionResult> AddMonsterBattle(MonsterBattleCreateDto mb)
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
                        isFinalized="false",
                        playeri=mb.PlayerId,
                        monsteri=mb.MonsterId
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
        
        [HttpPut("UpdateMonsterBattle")]
        public async Task<IActionResult> UpdateMonsterBattle(MonsterBattleUpdateDto monsterBattle)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                     var parameters = new 
                    {   monsterBattleId = monsterBattle.MonsterBattleId,
                        endedAt = monsterBattle.EndedAt,
                        isFinalized= "true",
                        lootItems=monsterBattle.LootItemsNames
                    };
                    if (monsterBattle.LootItemsNames.Length > 0){
                        var lootQuery =@"
                            MATCH (lootItem: Item)
                                WHERE lootItem.name
                                    IN $lootItems
                                    AND (lootItem)<-[:POSSIBLE_LOOT]-()
                            return lootItem"
                        ;
                        var lootItemResult = await session.RunAsync(lootQuery, parameters);
                        var lootItems = await lootItemResult.ToListAsync();

                        if (lootItems.Count != monsterBattle.LootItemsNames.Length)
                            throw new Exception("Some of the items don't exist");
                    }
                    var query = @"
                        MATCH (monsterBattle:MonsterBattle) WHERE ID(monsterBattle)=$monsterBattleId
                            SET monsterBattle.endedAt= $endedAt
                            SET monsterBattle.isFinalized= $isFinalized
                        WITH monsterBattle

                        MATCH (lootItem: Item)
                            WHERE lootItem.name
                            IN $lootItems
                        WITH monsterBattle, collect(lootItem) as lootItemsList            
                                    
                        FOREACH (item IN lootItemsList |
                                CREATE (monsterBattle)-[:LOOT]->(item)
                        )"     
                    ;
                    await session.RunAsync(query, parameters);
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }
           
        [HttpDelete("DeleteMonsterBattle")]
        public async Task<IActionResult> RemoveMonsterBattle(int monsterBattleId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
                    var query = @"MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(n2:Player) WHERE id(n)=$mbId DETACH DELETE n";
                    var parameters = new { mbId = monsterBattleId};
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
                return BadRequest(ex.Message);
            }
        }
        
        #endregion Loot
    }
}