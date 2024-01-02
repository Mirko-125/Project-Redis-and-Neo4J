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
        public async Task<IActionResult> AddMonster(Monster monster)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Monster {
                            name: $name,
                            zone: $zone,
                            type: $type,
                            imageUrl: $imageUrl,
                            status: $status
                        })";

                    var parameters = new
                    {
                        name = monster.Name,
                        zone = monster.Zone,
                        type = monster.Type,
                        imageUrl= monster.ImageURL,
                        status=monster.Status
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
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetMonster")]
        public async Task<IActionResult> GetMonster(string monsterName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Monster {name: $name}) RETURN n";
                        var parameters = new { name = monsterName };
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
        [HttpDelete("DeleteMonster")]
        public async Task<IActionResult> RemoveMonster(String monsterName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n:Monster {name: $name}) DETACH DELETE n";
                    var parameters = new { name = monsterName };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
       
        [HttpDelete("Delete")]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n) WHERE id(n)=$idn DETACH DELETE n";
                    var parameters = new { idn = id };
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
         [HttpPost("AddMonsterAttributes")]
        public async Task<IActionResult> AddMonsterAttributes(string monsterName,Attributes at)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                     var query = @"
                            CREATE (n:MonsterAttributes{
                            strength: $strength,
                            agility: $agility,
                            inteligence: $inteligence,
                            stamina: $stamina,
                            faith: $faith,
                            experience: $experience,
                            level: $level
                        })
                        WITH n
                        MATCH (n1:Monster {name: $name})
                        CREATE (n1)-[:HAS]->(n)";
                    var parameters = new {  
                        strength = at.Strength,
                        agility = at.Agility,
                        inteligence= at.Inteligence,
                        stamina=at.Stanima,
                        faith=at.Faith,
                        experience=at.Experience,
                        level=at.Level,
                        name = monsterName};
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
        public async Task<IActionResult> GetMonsterAttributes(string monsterName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Monster {name: $name})-[:HAS]->(n1:MonsterAttributes) RETURN n1 as osobine";
                        var parameters = new { name = monsterName };
                        var cursor = await tx.RunAsync(query,parameters);
                        /*var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });*/
                        var n=await cursor.SingleAsync();
                        var node = n["osobine"].As<INode>();
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
        [HttpDelete("DeleteMonsterAttributes")]
        public async Task<IActionResult> DeleteMonsterAttributes(string monsterName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                { 
                    var query = "MATCH (n:Monster {name: $name})-[:HAS]->(n1:MonsterAttributes) DETACH DELETE n1";
                    var parameters = new { name = monsterName };
                    await session.RunAsync(query,parameters);         
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion MonsterAttributes
    }

}