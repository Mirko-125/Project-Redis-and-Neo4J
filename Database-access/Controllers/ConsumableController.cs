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
    public class ConsumableController : ControllerBase
    {
        private readonly IDriver _driver;

        public ConsumableController(IDriver driver)
        {
            _driver = driver;
        }

     

        [HttpPost("AddConsumable")]
        public async Task<IActionResult> AddConsumable(Consumable consumable)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Consumable:Item {
                            name: $name,
                            weight: $weight,
                            type: $type,
                            dimensions: $dimensions,
                            value: $value,
                            effect: $effect
                        })";

                    var parameters = new
                    {
                        name=consumable.Name,
                        weight=consumable.Weight,
                        type=consumable.Type,
                        dimensions=consumable.Dimensions,
                        value=consumable.Value,
                        effect=consumable.Effect
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
           
        [HttpPut("UpdateConsumable")]
        public async Task<IActionResult> UpdateConsumable(string name, int value, int dimensions, double weight, string effects)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Consumable {name: $name}) 
                                    SET n.value=$value 
                                    SET n.dimensions=$dimensions 
                                    SET n.weight=$weight 
                                    SET n.effects=$effects 
                                    return n";
                    var parameters = new { name=name, 
                                           value=value, 
                                           dimensions=dimensions, 
                                           weight=weight, 
                                           effects=effects };
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