using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using ServiceStack.Text;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketplaceController : ControllerBase
    {
        private readonly IDriver _driver;
        private readonly IRedisClientsManager _redisClientsManager;

        public MarketplaceController(IDriver driver, IRedisClientsManager redisClientsManager)
        {
            _redisClientsManager = redisClientsManager;
            _driver = driver;
        }

        [HttpGet("GetAllMarketplaces")]
        public async Task<IActionResult> GetAllMarketplaces()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var redisClient = await _redisClientsManager.GetClientAsync();
                    var keyExists = await redisClient.ContainsKeyAsync("marketplaces");
                    if (keyExists)
                    {
                        var json = await redisClient.GetAsync<string>("marketplaces");
                        return Ok(JsonSerializer.DeserializeFromString<List<Marketplace>>(json));
                    }
                    
                    var query = "MATCH (n:Marketplace)-[:HAS]->(i:Item) RETURN n, COLLECT(i) as items";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<object>();

                    await cursor.ForEachAsync(record =>
                    {
                        var marketPlace = record["n"].As<INode>();
                        var connectedNodes = record["items"].As<INode>();
                        resultList.Add(new { MarketPlace = marketPlace, Items = connectedNodes });
                    });
                    var marketplaceJson = JsonSerializer.SerializeToString(resultList);
                    await redisClient.SetAsync("marketplace", marketplaceJson);

                    return Ok(resultList);
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