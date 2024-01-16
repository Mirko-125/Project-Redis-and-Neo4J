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
        //dodati route lvlup(player) vuce LevelGainAttrbiutes od klase igraca, i dodaje mu u njegove atribute
        [HttpPut("LevelUp")]
        public async Task<ActionResult> LevelUp(int playerId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (player:Player) WHERE ID(player)=$id
                                  MATCH (player)-[IS]->(class)
                                  MATCH (class)-[LEVEL_GAINS_ATTRIBUTES]->(x)
                                  MATCH (player)-[HAS]->(attributes)
                                    SET attributes.strength = attributes.strength + x.strength,
                                        attributes.agility = attributes.agility + x.agility,
                                        attributes.intelligence = attributes.intelligence + x.intelligence,
                                        attributes.stamina = attributes.stamina + x.stamina,
                                        attributes.faith = attributes.faith + x.faith,
                                        attributes.experience = attributes.experience + 25,
                                        attributes.level = attributes.level + 1";
                    var parameters = new 
                    { 
                        id = playerId
                    };
    
                    var result = await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Ubaciti i klasu kao parameter u dto, i koristiti baseAttributes od te klase da se generisu atributi playera
        [HttpPost("AddProperPlayer")]
        public async Task<IActionResult> AddProperPlayer(PlayerDto player)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {

                    var query = @"CREATE (n:Player { name: $name, email: $email, bio: $bio, achievementPoints: 0, createdAt: $createdAt, password: $password, gold: 0, honor: 0})
                                    WITH n
                                    MATCH (class:Class) WHERE ID(class)=$classId
                                    CREATE (n)-[:IS]->(class)
                                    WITH n, class
                                    MATCH (class)-[:HAS_BASE_ATTRIBUTES]->(x)
                                    CREATE (m:Inventory {weightLimit : 0, dimensions: 0, freeSpots: 0, usedSpots: 0})
                                    CREATE (o:Attributes { strength: x.strength, agility: x.agility, intelligence: x.intelligence, stamina: x.stamina, faith: x.faith, experience: 0, level: 0})
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
                        password = player.Password,
                        classId = player.ClassId
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