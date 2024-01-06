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
                        CREATE (n:Consumable {
                            effect: $effect
                        }) 
                        CREATE(m:Item {
                            name: $name,
                            weight: $weight,
                            type: $type,
                            dimensions: $dimensions,
                            value: $value
                        })
                        
                        CREATE (n)-[:IS_A]->(m)";

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
        public async Task<IActionResult> UpdateConsumable(String name, int value, int dimensions, double weight, String effects)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Item {name: $name}) SET n.value=$value, n.dimensions=$dimensions, n.weight=$weight, n.effects=$effects return n";
                    var parameters = new { name=name, value=value, dimensions=dimensions, weight=weight, effects=effects};
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