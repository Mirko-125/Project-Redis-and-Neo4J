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
    public class GearController : ControllerBase
    {
        private readonly IDriver _driver;

        public GearController(IDriver driver)
        {
            _driver = driver;
        }


        [HttpPost("AddGear")]
        public async Task<IActionResult> AddGear(GearCreateDto gear)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (gear:Item:Gear {
                            name: $name,
                            weight: $weight,
                            type: $type,
                            dimensions: $dimensions,
                            value: $value,
                            slot: $slot,
                            level: $level,
                            quality: $quality
                        })
                        CREATE (attributes:Attributes { 
                            strength: $strength, 
                            agility: $agility, 
                            intelligence: $intelligence, 
                            stamina: $stamina, 
                            faith: $faith, 
                            experience: $experience, 
                            levelAttributes: $levelAttributes
                        })
                        CREATE (gear)-[:HAS]->(attributes)";

                    var parameters = new
                    {
                        name = gear.Name,
                        weight = gear.Weight,
                        type = gear.Type,
                        dimensions = gear.Dimensions,
                        value = gear.Value,
                        slot = gear.Slot,   
                        level = gear.Level,
                        quality = gear.Quality,

                        //attributes
                        strength = gear.Attributes.Strength,
                        agility = gear.Attributes.Agility,
                        intelligence = gear.Attributes.Intelligence,
                        stamina = gear.Attributes.Stamina,
                        faith = gear.Attributes.Faith,
                        experience = gear.Attributes.Experience,
                        levelAttributes = gear.Attributes.Level
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
           
        [HttpPut("UpdateGear")]
        public async Task<IActionResult> UpdateGear(GearUpdateDto gear)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Gear)-[:HAS]->(attributes:Attributes) WHERE ID(n)=$gearID
                                    SET n.name=$name
                                    SET n.type=$type
                                    SET n.value=$value 
                                    SET n.dimensions=$dimensions 
                                    SET n.weight=$weight 
                                    SET n.slot=$slot
                                    SET n.level=$level
                                    SET n.quality=$quality

                                    SET attributes.strength= $strength
                                    SET attributes.agility= $agility
                                    SET attributes.intelligence= $intelligence
                                    SET attributes.stamina= $stamina
                                    SET attributes.faith= $faith
                                    SET attributes.experience= $experience
                                    SET attributes.levelAttributes= $levelAttributes

                                    return n";
                    var parameters = new 
                    { 
                        gearID = gear.GearID,
                        name = gear.Name,
                        type = gear.Type, 
                        value = gear.Value, 
                        dimensions = gear.Dimensions, 
                        weight = gear.Weight, 
                        slot = gear.Slot,
                        level = gear.Level,
                        quality = gear.Quality,

                        strength = gear.Attributes.Strength,
                        agility = gear.Attributes.Agility ,
                        intelligence = gear.Attributes.Intelligence,
                        stamina = gear.Attributes.Stamina,
                        faith = gear.Attributes.Faith,
                        experience = gear.Attributes.Experience ,
                        levelAttributes = gear.Attributes.Level                
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
        
    }
}