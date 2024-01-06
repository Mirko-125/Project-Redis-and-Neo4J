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

        [HttpGet("GetAllMarketplaces")]
        public async Task<IActionResult> GetAllMarketplaces()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Marketplace) RETURN n";
                        var cursor = await tx.RunAsync(query);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetMarketplace")]
        public async Task<IActionResult> GetMarketplace(String zone)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Marketplace {zone: $zone}) RETURN n";
                        var parameters = new { zone = zone };
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddMarketplace")]
        public async Task<IActionResult> AddMarketplace(Marketplace marketplace)
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
        

        [HttpDelete("DeleteMarketplace")]
        public async Task<IActionResult> DeleteMarketplace(String zone)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Marketplace {zone: $zone}) DELETE n";
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

        [HttpPut("UpdateMarketplace")]
        public async Task<IActionResult> UpdateMarketplace(String zone, int itemCount)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Marketplace {zone: $zone}) set n.itemCount=$itemCount return n";
                    var parameters = new { itemCount = itemCount, zone=zone };
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
        public async Task<IActionResult> AddMarketplaceItems(String zone, Item item)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                     var query = @"
                        CREATE (n:Item {
                            name: $name,
                            weight: $weight,
                            type: $type,
                            dimensions: $dimensions,
                            value: $value
                        })
                        WITH n
                        MATCH (n1:Marketplace {zone: $zone})
                        CREATE (n1)-[:HAS]->(n)";
                    var parameters = new {  
                        name = item.Name,
                        weight = item.Weight,
                        type = item.Type,
                        dimensions = item.Dimensions,
                        value = item.Value,
                        zone=zone
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
        

        [HttpGet("GetMarketplaceItems")]
        public async Task<IActionResult> GetMarketplaceItems(String zone)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Item)<-[:HAS]-(n1:Marketplace {zone: $zone}) RETURN n";
                                     
                        
                        var parameters = new { zone = zone };
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
   
   
   
    }
}