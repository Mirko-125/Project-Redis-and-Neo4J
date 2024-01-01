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
    public class PlayerController : ControllerBase
    {
        private readonly IDriver _driver;

        public PlayerController(IDriver driver)
        {
            _driver = driver;
        }

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
                        name = player.Name,
                        email = player.Email,
                        bio = player.Bio,
                        achievementPoints = player.AchievementPoints,
                        createdAt = player.CreatedAt,
                        password = player.Password,
                        gold = player.Gold,
                        honor = player.Honor
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
    }
}