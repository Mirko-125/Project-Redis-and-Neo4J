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
        public async Task<IActionResult> AddConsumable(ConsumableCreateDto consumable)
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
        public async Task<IActionResult> UpdateConsumable(ConsumableUpdateDto consumable)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Consumable) WHERE ID(n)=$consumableID
                                    SET n.name=$name
                                    SET n.type=$type
                                    SET n.value=$value 
                                    SET n.dimensions=$dimensions 
                                    SET n.weight=$weight 
                                    SET n.effect=$effect 
                                    return n";
                    var parameters = new { consumableID=consumable.ConsumableID,
                                           name=consumable.Name, 
                                           type=consumable.Type,
                                           value=consumable.Value, 
                                           dimensions=consumable.Dimensions, 
                                           weight=consumable.Weight, 
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
    
    
   
        
    }
}