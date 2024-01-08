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
        public async Task<IActionResult> AddGear(GearDto gear)
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
                            inteligence: $inteligence, 
                            stamina: $stamina, 
                            faith: $faith, 
                            experience: $experience, 
                            levelAttributes: $levelAttributes
                        })
                        CREATE (gear)-[:HAS]->(attributes)";

                    var parameters = new
                    {
                        name=gear.Name,
                        weight=gear.Weight,
                        type=gear.Type,
                        dimensions=gear.Dimensions,
                        value=gear.Value,
                        slot=gear.Slot,   
                        level=gear.Level,
                        quality=gear.Quality,

                        //attributes
                        strength = gear.Attributes.Strength,
                        agility = gear.Attributes.Agility,
                        inteligence = gear.Attributes.Inteligence,
                        stamina = gear.Attributes.Stanima,
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
        public async Task<IActionResult> UpdateGear(string name, int value, int dimensions, double weight, int slot, int level, string quality)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Gear {name: $name}) 
                                    SET n.value=$value 
                                    SET n.dimensions=$dimensions 
                                    SET n.weight=$weight 
                                    SET n.slot=$slot
                                    SET n.level=$level
                                    SET n.quality=$quality 
                                    return n";
                    var parameters = new { name=name, 
                                           value=value, 
                                           dimensions=dimensions, 
                                           weight=weight, 
                                           slot=slot,
                                           level=level,
                                           quality=quality };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
        [HttpPut("UpdateGearAttributes")]
        public async Task<IActionResult> UpdateGearAttributes(string name, double strength, double agility, double inteligence, double stamina, double faith, double experience, int level)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (gear:Gear {name: $name})-[:HAS]->(attributes:Attributes)
                                SET attributes.strength= $strength
                                SET attributes.agility= $agility
                                SET attributes.inteligence= $inteligence
                                SET attributes.stamina= $stamina
                                SET attributes.faith= $faith
                                SET attributes.experience= $experience
                                SET attributes.level= $level
                                RETURN attributes";
                    var parameters = new { name=name,
                                        strength = strength,
                                        agility=agility ,
                                        inteligence=inteligence,
                                        stamina= stamina,
                                        faith= faith,
                                        experience=experience ,
                                        level=level };
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