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
    public class ClassController : ControllerBase
    {
        private readonly IDriver _driver;

        public ClassController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("CreateClass")]
        public async Task<IActionResult> CreateClass(ClassDto classEntity)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (class:Class { name: $nameValue})
                        CREATE (base:Attributes { strength: $strengthValue, agility: $agilityValue, intelligence: $intelligenceValue, stamina: $staminaValue, faith: $faithValue, experience: -1, level: -1})
                        CREATE (level:Attributes { strength: $strength, agility: $agility, intelligence: $intelligence, stamina: $stamina, faith: $faith, experience: -1, level: -1})
                        CREATE (class)-[:HAS_BASE_ATTRIBUTES]->(base)
                        CREATE (class)-[:LEVEL_GAINS_ATTRIBUTES]->(level)";

                    var parameters = new
                    {
                        // Value is for the base attributes
                        // Just a name is for the level gains attributes
                        nameValue = classEntity.Name,
                        strengthValue = classEntity.BaseAttributes.Strength,
                        agilityValue = classEntity.BaseAttributes.Agility,
                        intelligenceValue = classEntity.BaseAttributes.Intelligence,
                        staminaValue = classEntity.BaseAttributes.Stamina,
                        faithValue = classEntity.BaseAttributes.Faith,
                        strength = classEntity.LevelGainAttributes.Strength,
                        agility = classEntity.LevelGainAttributes.Agility,
                        intelligence = classEntity.LevelGainAttributes.Intelligence,
                        stamina = classEntity.LevelGainAttributes.Stamina,
                        faith = classEntity.LevelGainAttributes.Faith
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

        [HttpPost("ClassPermission")]
        public async Task<IActionResult> ClassPermission(int classId, int abilityId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        MATCH (n:Class) WHERE ID(n)=$cId
                        MATCH (m:Ability) WHERE ID(m)=$aId
                        CREATE (n)-[:PERMITS]->(m)";

                    var parameters = new
                    {
                        cId = classId,
                        aId = abilityId
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

        [HttpDelete]
        public async Task<IActionResult> RemoveClass(int classId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        MATCH (c:Class) where ID(c)=$cId
                            OPTIONAL MATCH (c)-[r]-()
                        DELETE r,c";

                    var parameters = new { cId = classId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateClass")]
        public async Task<IActionResult> UpdateClass(int classId, string newName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        MATCH (n:Class) WHERE ID(n)=$cId
                            SET n.name=$name
                        RETURN n";

                    var parameters = new { 
                        cId = classId,
                        name = newName 
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

        [HttpGet("GetAllClasses")]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = "MATCH (n:Class) RETURN n";
                    var cursor = await session.RunAsync(query);
                    var classList = new List<Class>();

                    await cursor.ForEachAsync(record =>
                    {
                        var node = record["n"].As<INode>();
                        Class cls = new Class(node);
                        classList.Add(cls);
                    });
                    
                    return Ok(classList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}