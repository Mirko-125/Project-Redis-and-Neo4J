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
    public class InventoryController : ControllerBase
    {
        private readonly IDriver _driver;

        public InventoryController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"CREATE (n:Inventory { weightlimit: $weightlimit,
                                                        dimensions: $dimensions,
                                                        freespots: $freespots,
                                                        usedspots: $usedspots
                                        })";

                    var parameters = new
                    {
                        weightlimit = inventory.WeightLimit,
                        dimensions = inventory.Dimensions,
                        freespots = inventory.FreeSpots,
                        usedspots = inventory.UsedSpots,
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
        [HttpPut]
        public async Task<IActionResult> ConnectInventory(int inventoryId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Player {id: $id-1}), (m:Inventory {id: $id})
                                        MERGE (n)-[:HAS]->(m)";

                    var parameters = new
                    {
                        id = inventoryId
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
        // [HttpDelete]
        // removing player would remove the inventory?
    }
}