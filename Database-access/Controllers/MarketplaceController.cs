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
    public class MarketplaceController : ControllerBase
    {
        private readonly IDriver _driver;

        public MarketplaceController(IDriver driver)
        {
            _driver = driver;
        }


        [HttpPost("AddMarketplace")]
        public async Task<IActionResult> AddMarketplace(MarketplaceDto marketplace)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:Marketplace {
                            zone: $zone,
                            itemCount: $itemCount,
                            restockCycle: $restockCycle
                        })";

                    var parameters = new
                    {
                        zone = marketplace.Zone,
                        itemCount = marketplace.ItemCount,
                        restockCycle = marketplace.RestockCycle
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
        
        [HttpPost("AddMarketplaceItems")]
        public async Task<IActionResult> AddMarketplaceItems(string zoneName, string itemName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                     var query = @"
                        MATCH (n:Item)
                            WHERE n.name = $item
                        WITH n
                        MATCH (n1:Marketplace {zone: $zone})
                        CREATE (n1)-[:HAS]->(n)";
                    var parameters = new {  
                        item = itemName,
                        zone = zoneName
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
        
        [HttpGet("GetAllMarketplaces")]
        public async Task<IActionResult> GetAllMarketplaces()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Marketplace)-[:HAS]->(i:Item) RETURN n, COLLECT(i) as items";
                        var cursor = await session.RunAsync(query);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var item = record["n"].As<INode>();
                            var connectedNodes = record["items"].As<INode>();
                            resultList.Add(new { Item = item, ConnectedNodes = connectedNodes });
                        });

                        return resultList;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
   
        [HttpGet("GetMarketplace")]
        public async Task<IActionResult> GetMarketplace(string zone)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Marketplace {zone: $zone})-[:HAS]->(i:Item) RETURN n, i";
                        var parameters = new { zone = zone };
                        var cursor = await tx.RunAsync(query,parameters);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var item = record["n"].As<INode>();
                            var connectedNodes = record["i"].As<INode>();
                            resultList.Add(new { Item = item, ConnectedNodes = connectedNodes });
                        });

                        return resultList;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateMarketplace")]
        public async Task<IActionResult> UpdateMarketplace(string zone, int itemCount, int restockCycle)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Marketplace {zone: $zone})
                                  SET n.itemCount=$itemCount
                                  SET n.restockCycle=$restockCycle
                                  return n";
                    var parameters = new { itemCount = itemCount, 
                                           zone=zone,
                                           restockCycle=restockCycle };
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     
        [HttpDelete("DeleteMarketplace")]
        public async Task<IActionResult> DeleteMarketplace(string zone)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Marketplace {zone: $zone}) DETACH DELETE n";
                    var parameters = new { zone = zone };
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