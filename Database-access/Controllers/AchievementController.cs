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
    public class AchievementController : ControllerBase
    {
        private readonly IDriver _driver;

        public AchievementController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet("AllAchievements")]
        public async Task<IActionResult> AllAchievements()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Achievement) RETURN n";
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

        [HttpPost("AddAchievement")]
        public async Task<IActionResult> AddAchievement(Achievement achievement)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Achievement {
                            name: $name,
                            type: $type,
                            points: $points,
                            conditions: $conditions
                        })";

                    var parameters = new
                    {
                        name = achievement.Name,
                        points = achievement.Points,
                        type = achievement.Points,
                        conditions = achievement.Conditions
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

        [HttpPost("GiveAchievement")]
        public async Task<IActionResult> GiveAchievement(int playerId, int achievementId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (p:Player) where ID(p)=$pId
                                MATCH (a:Achievement) where ID(a)=$aId
                                CREATE (p)-[:ACHIEVED]->(a)";

                    var parameters = new
                    {
                        pId = playerId,
                        aId = achievementId
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
        public async Task<IActionResult> RemoveAchievement(int achievementId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (a:Achievement) where ID(a)=$aId
                                OPTIONAL MATCH (a)-[r]-()
                                DELETE r,a";
                    var parameters = new { aId = achievementId };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateAchievement")]
        public async Task<IActionResult> UpdateAchievement(int achievementId, string newName, string newType, int newPoints, string newConditions)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Achievement) WHERE ID(n)=$aId
                                SET n.name=$name
                                SET n.type=$type
                                SET n.points=$points
                                SET n.conditions=$conditions
                                RETURN n";
                    var parameters = new { aId = achievementId,
                                        name = newName,
                                        type = newType,
                                        points = newPoints,
                                        conditions = newConditions };
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