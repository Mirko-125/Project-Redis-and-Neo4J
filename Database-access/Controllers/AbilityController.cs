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
    public class AbilityController : ControllerBase
    {
        private readonly IDriver _driver;

        public AbilityController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("CreateAbility")]
        public async Task<IActionResult> CreateAbility(Ability ability)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:Ability { name: $name, 
                                                    damage: $damage,
                                                    cooldown: $cooldown,
                                                    range: $range,
                                                    special: $special,
                                                    heal: $heal
                                        })";

                    var parameters = new
                    {
                        name = ability.Name,
                        damage = ability.Damage,
                        cooldown = ability.Cooldown,
                        range = ability.Range,
                        special = ability.Special,
                        heal = ability.Heal 
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
        [HttpPost("AssignAbility")]
        public async Task<IActionResult> AssignAbility(int abilityId, int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Ability) WHERE ID(n)=$aId
                                MATCH (m:Player) WHERE ID(m)=$pId
                                CREATE (m)-[:KNOWS]->(n)";

                    var parameters = new
                    {
                        aId = abilityId,
                        pId = playerId
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
        public async Task<IActionResult> RemoveAbility(int abilityId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (c:Ability) where ID(c)=$aId
                                OPTIONAL MATCH (c)-[r]-()
                                DELETE r,c";
                    var parameters = new { aId = abilityId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateAbility")]
        public async Task<IActionResult> UpdateAbility(int abilityId, string newName ,int newDamage, int newCooldown, double newRange, string newSpecial, int newHeal)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Ability) WHERE ID(n)=$aId
                                SET n.name=$name
                                SET n.damage=$damage
                                SET n.cooldown=$cooldown
                                SET n.range=$range
                                SET n.special=$special
                                SET n.heal=$heal
                                RETURN n";
                    var parameters = new { aId = abilityId,
                                        name = newName,
                                        damage = newDamage,
                                        cooldown = newCooldown,
                                        range = newRange,
                                        special = newSpecial,
                                        heal = newHeal };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("AllAbilites")]
        public async Task<IActionResult> AllAbilites()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Ability) RETURN n";
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
    }
}