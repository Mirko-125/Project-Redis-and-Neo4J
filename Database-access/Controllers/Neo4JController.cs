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
    public class Neo4jController : ControllerBase
    {
        private readonly IDriver _driver;

        public Neo4jController(IDriver driver)
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
                    var result = await session.ReadTransactionAsync(async tx =>
                    {
                        var query = "MATCH (n) RETURN n";
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
        #region Ship to Player

        [HttpPost]
        public async Task<IActionResult> AddPlayer(Player player)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Player {
                            name: $name,
                            email: $email,
                            bio: $bio,
                            achievementPoints: $achievementPoints,
                            createdAt: $createdAt,
                            password: $password,
                            gold: $gold,
                            honor: $honor
                        })";

                    var parameters = new
                    {
                        name = player.name,
                        email = player.email,
                        bio = player.bio,
                        achievementPoints = player.achievementPoints,
                        createdAt = player.createdAt,
                        password = player.password,
                        gold = player.gold,
                        honor = player.honor
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

        #region Ship to Player

        [HttpDelete]
        public async Task<IActionResult> RemovePlayer(String playerName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Player {name: $name}) DELETE n";
                    var parameters = new { name = playerName };
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