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
                        CREATE 
                        (n:Player { name: $name, email: $email, bio: $bio,
                                    achievementPoints: $achievementPoints, 
                                    createdAt: $createdAt, password: $password, 
                                    gold: $gold, honor: $honor})
                                    -[:OWNS]->
                                    (m {weightLimit : $weightLimit, dimensions: $dimensions, freeSpots: $freeSpots, usedSpots: $usedSpots}), 
                                    ((n)-[HAS]->(o:Attributes { strength: $strength, agility: $agility, 
                                                                inteligence: $inteligence, stamina: $stamina, faith: $faith, experience: $experience, level: $level})), 
                                    ((n)-[WEARS]->(p:Equipment { averageQuality: $averageQuality, weight: $weight}))
                        ";

                    var parameters = new
                    {
                        name = player.Name,
                        email = player.Email,
                        bio = player.Bio,
                        achievementPoints = player.AchievementPoints,
                        createdAt = player.CreatedAt,
                        password = player.Password,
                        gold = player.Gold,
                        honor = player.Honor,
                        // Inventory
                        weightLimit = player.Inventory.WeightLimit,
                        dimensions = player.Inventory.Dimensions,
                        freeSpots = player.Inventory.FreeSpots,
                        usedSpots = player.Inventory.UsedSpots,
                        // Attributes
                        strength = player.Attributes.Strength,
                        agility = player.Attributes.Agility,
                        inteligence = player.Attributes.Inteligence,
                        stamina = player.Attributes.Stanima,
                        faith = player.Attributes.Faith,
                        experience = player.Attributes.Experience,
                        level = player.Attributes.Level,
                        // Equipment
                        averageQuality = player.Equipment.FirstOrDefault().AverageQuality,
                        weight = player.Equipment.FirstOrDefault().Weight
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