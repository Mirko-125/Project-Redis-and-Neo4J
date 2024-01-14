using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using System.Reflection.Metadata.Ecma335;

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

        [HttpPost("AddFullPlayer")]
        public async Task<IActionResult> AddPlayer(Player player)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:Player { name: $name, email: $email, bio: $bio, achievementPoints: $achievementPoints, createdAt: $createdAt, password: $password, gold: $gold, honor: $honor})
                                CREATE (m:Inventory {weightLimit : $weightLimit, dimensions: $dimensions, freeSpots: $freeSpots, usedSpots: $usedSpots})
                                CREATE (o:Attributes { strength: $strength, agility: $agility, inteligence: $inteligence, stamina: $stamina, faith: $faith, experience: $experience, level: $level})
                                CREATE (p:Equipment { averageQuality: $averageQuality, weight: $weight})
                                CREATE (n)-[:OWNS]->(m)
                                CREATE (n)-[:HAS]->(o)
                                CREATE (n)-[:WEARS]->(p)";

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
                        averageQuality = player.Equipment.AverageQuality,
                        weight = player.Equipment.Weight
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

        [HttpPost("AddProperPlayer")]
        public async Task<IActionResult> AddProperPlayer(PlayerDto player)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:Player { name: $name, email: $email, bio: $bio, achievementPoints: 0, createdAt: $createdAt, password: $password, gold: 0, honor: 0})
                                CREATE (m:Inventory {weightLimit : 0, dimensions: 0, freeSpots: 0, usedSpots: 0})
                                CREATE (o:Attributes { strength: 0, agility: 0, inteligence: 0, stamina: 0, faith: 0, experience: 0, level: 0})
                                CREATE (p:Equipment { averageQuality: 0, weight: 0})
                                CREATE (n)-[:OWNS]->(m)
                                CREATE (n)-[:HAS]->(o)
                                CREATE (n)-[:WEARS]->(p)";

                    var parameters = new
                    {
                        name = player.Name,
                        email = player.Email,
                        bio = player.Bio,
                        createdAt = player.CreatedAt,
                        password = player.Password
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

        [HttpPut("AddItem")]
        public async Task<IActionResult> AddItem(string itemName, int playerID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new {
                        item = itemName,
                        player = playerID
                    };

                    var query =@"
                        MATCH (item:Item) 
                            WHERE item.name = $item

                        WITH item
                        MATCH (player:Player)-[:OWNS]->(inventory:Inventory)
                            WHERE ID(player) = $player
                        
                        CREATE (inventory)-[:HAS]->(item)"
                    ;
                    var result = await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> RemovePlayer(int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (p:Player) WHERE ID(p)=$id
                                OPTIONAL MATCH (p)-[r]->(otherSide)
                                DELETE r,p,otherSide"; 
                    var parameters = new { id = playerId };
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