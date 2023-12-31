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
    public class MonsterController : ControllerBase
    {
        private readonly IDriver _driver;

        public MonsterController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost("AddMonster")]
        public async Task<IActionResult> AddMonster(MonsterCreateDto monster)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new
                    {
                        name = monster.Name,
                        zone = monster.Zone,
                        type = monster.Type,
                        imageUrl= monster.ImageURL,
                        status=monster.Status,
                        strength = monster.Attributes.Strength,
                        agility = monster.Attributes.Agility,
                        inteligence= monster.Attributes.Inteligence,
                        stamina=monster.Attributes.Stanima,
                        faith=monster.Attributes.Faith,
                        experience=monster.Attributes.Experience,
                        level=monster.Attributes.Level,
                        possibleLootItems=monster.PossibleLootNames
                    };
                    if (monster.PossibleLootNames.Length > 0){
                        var possibleLootQuery =@"
                            MATCH (possibleLootItem: Item)
                                WHERE possibleLootItem.name
                                    IN $possibleLootItems

                            return possibleLootItem"
                        ;
                        var possibleLootItemResult = await session.RunAsync(possibleLootQuery, parameters);
                        var possibleLootItems = await possibleLootItemResult.ToListAsync();

                        if (possibleLootItems.Count != monster.PossibleLootNames.Length)
                            throw new Exception("Some of the items don't exist");
                    }
                    var query = @"
                        CREATE (monster:Monster {
                            name: $name,
                            zone: $zone,
                            type: $type,
                            imageUrl: $imageUrl,
                            status: $status
                        })
                        CREATE (monsterAttributes:MonsterAttributes {
                            strength: $strength,
                            agility: $agility,
                            inteligence: $inteligence,
                            stamina: $stamina,
                            faith: $faith,
                            experience: $experience,
                            level: $level
                        })
                        CREATE (monster)-[:HAS]->(monsterAttributes)
                        WITH monster

                        MATCH (possibleLootItem: Item)
                                WHERE possibleLootItem.name
                                    IN $possibleLootItems
                        WITH monster,collect(possibleLootItem) as possibleLootItemsList            
                                    
                        FOREACH (item IN possibleLootItemsList |
                            CREATE (monster)-[:POSSIBLE_LOOT]->(item)
                        )";
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("UpdateMonster")]
        public async Task<IActionResult> UpdateMonster(MonsterUpdateDto monster)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Monster) WHERE ID(n)=$mId
                                SET n.zone= $zone
                                SET n.imageUrl= $imageUrl
                                SET n.status= $status
                                RETURN n";
                    var parameters = new { mId = monster.MonsterId,
                                        zone = monster.Zone,
                                        imageUrl= monster.ImageURL,
                                        status=monster.Status };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllMonsters")]
        public async Task<IActionResult> GetAllMonsters()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Monster) RETURN n";
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
        
        [HttpGet("GetMonster")]
        public async Task<IActionResult> GetMonster(int monsterId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"MATCH (n:Monster) WHERE id(n)=$idn   
                                    RETURN n";
                        var parameters = new { idn = monsterId };
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
        
        [HttpDelete("DeleteMonster")]
        public async Task<IActionResult> RemoveMonster(int monsterId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {  
                    var query = @"MATCH (n:Monster)-[r]->(m:MonsterAttributes) WHERE id(n)=$idn
                                DETACH DELETE r,m,n";
                    var parameters = new { idn=monsterId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        #region MonsterAttributes

        [HttpPut("UpdateMonsterAttributes")]
        public async Task<IActionResult> UpdateMonsterAttributes(int monsterId, double newStrength, double newAgility, double newInteligence, double newStamina, double newFaith, double newExperience, int newLevel)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Monster)-[:HAS]->(ma:MonsterAttributes) WHERE ID(n)=$mId
                                SET ma.strength= $strength
                                SET ma.agility= $agility
                                SET ma.inteligence= $inteligence
                                SET ma.stamina= $stamina
                                SET ma.faith= $faith
                                SET ma.experience= $experience
                                SET ma.level= $level
                                RETURN ma";
                    var parameters = new { mId = monsterId,
                                        strength = newStrength,
                                        agility=newAgility ,
                                        inteligence=newInteligence,
                                        stamina= newStamina,
                                        faith= newFaith,
                                        experience=newExperience ,
                                        level=newLevel };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMonsterAttributes")]
        public async Task<IActionResult> GetMonsterAttributes(int monsterId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Monster)-[:HAS]->(n1:MonsterAttributes) WHERE id(n)=$mid RETURN n1 as osobine";
                        var parameters = new { mid = monsterId };
                        var cursor = await tx.RunAsync(query,parameters);
                        var n=await cursor.SingleAsync();
                        var node = n["osobine"].As<INode>();
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
        
        [HttpDelete("DeleteMonsterAttributes")]
        public async Task<IActionResult> DeleteMonsterAttributes(int monsterId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                { 
                    var query = @"MATCH (n:Monster)-[:HAS]->(n1:MonsterAttributes) WHERE id(n)=$mid 
                                DETACH DELETE n1";
                    var parameters = new { mid = monsterId };
                    await session.RunAsync(query,parameters);         
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion MonsterAttributes
    
        #region PossibleLoot

        [HttpGet("GetPossibleLoot")]
        public async Task<IActionResult> GetPossibleLoot(int monsterId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Monster)-[:POSSIBLE_LOOT]->(n1:Item) WHERE id(n)=$id RETURN n1";
                        var parameters = new{id=monsterId};
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
        
        #endregion PossibleLoot
    }

}