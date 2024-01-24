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
                        imageURL= monster.ImageURL,
                        status=monster.Status,
                        strength = monster.Attributes.Strength,
                        agility = monster.Attributes.Agility,
                        intelligence= monster.Attributes.Intelligence,
                        stamina=monster.Attributes.Stamina,
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
                            imageURL: $imageURL,
                            status: $status
                        })
                        CREATE (monsterAttributes:Attributes {
                            strength: $strength,
                            agility: $agility,
                            intelligence: $intelligence,
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
                    var query = @"MATCH (n:Monster)-[:HAS]->(ma:Attributes) WHERE ID(n)=$mId
                                SET n.zone= $zone
                                SET n.imageURL= $imageURL
                                SET n.status= $status
                                SET ma.strength= $strength
                                SET ma.agility= $agility
                                SET ma.intelligence= $intelligence
                                SET ma.stamina= $stamina
                                SET ma.faith= $faith
                                SET ma.experience= $experience
                                SET ma.level= $level
                                RETURN n";
                    var parameters = new { 
                        mId = monster.MonsterId,
                        zone = monster.Zone,
                        imageURL= monster.ImageURL,
                        status = monster.Status,
                        strength = monster.Attributes.Strength,
                        agility = monster.Attributes.Agility ,
                        intelligence = monster.Attributes.Intelligence,
                        stamina = monster.Attributes.Stamina,
                        faith = monster.Attributes.Faith,
                        experience = monster.Attributes.Experience ,
                        level = monster.Attributes.Level 
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
        
        [HttpGet("GetAllMonsters")]
        public async Task<IActionResult> GetAllMonsters()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                   var query = @"MATCH (n:Monster)-[:HAS]->(m:Attributes) 
                                MATCH (n)-[:POSSIBLE_LOOT]->(i:Item)
                                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                                RETURN n, m, COLLECT({
                                    item: i,
                                    attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                                }) AS possibleLoot";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<Monster>();
                    await cursor.ForEachAsync(record =>
                    {
                        var monsterNode = record["n"].As<INode>();
                        var monsterAttributesNode = record["m"].As<INode>();
                        var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
                        Monster monster= new(monsterNode, possibleLootNodeList, monsterAttributesNode);
                        resultList.Add(monster);
                    });

                    return Ok(resultList);
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
                   var parameters = new { idn = monsterId };
                    var query = @"MATCH (n:Monster) WHERE id(n)=$idn
                                MATCH (n)-[:HAS]->(m:Attributes) 
                                MATCH (n)-[:POSSIBLE_LOOT]->(i:Item)
                                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                                RETURN n, m, COLLECT({
                                    item: i,
                                    attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                                }) AS possibleLoot ";
                    var cursor = await session.RunAsync(query, parameters);
                    var result=await cursor.SingleAsync();
                    var monsterNode = result["n"].As<INode>();
                    var monsterAttributesNode = result["m"].As<INode>();
                    var possibleLootNodeList = result["possibleLoot"].As<List<Dictionary<string, INode>>>();
                    Monster monster=new(monsterNode, possibleLootNodeList , monsterAttributesNode);
                    return Ok(monster);
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
                    var query = @"MATCH (n:Monster)-[r:HAS]->(m:Attributes) 
                                    WHERE Id(n)=$nId
                                OPTIONAL MATCH (n)<-[a:ATTACKED_MONSTER]-(mb:MonsterBattle)
                                DETACH DELETE m,mb,n";
                    var parameters = new { nId = monsterId };
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