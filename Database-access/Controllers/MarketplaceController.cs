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
        public async Task<IActionResult> AddMarketplace(MarketplaceCreateDto marketplace)
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
                        var query = @"
                            MATCH (n:Marketplace)-[:HAS]->(i:Item) 
                                OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                            RETURN n, COLLECT({
                                item: i,
                                attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                            }) AS items";
                        var cursor = await tx.RunAsync(query);
                        var resultList = new List<Marketplace>();
                        await cursor.ForEachAsync(record =>
                        {
                            var marketNode = record["n"].As<INode>();
                            var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
                            Marketplace market = new(marketNode, itemsNodeList);
                            resultList.Add(market);
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
                            var marketplace = record["n"].As<INode>();
                            var itemNodes = record["i"].As<INode>();
                            resultList.Add(new { Marketplace = marketplace, ItemNodes = itemNodes });
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
        public async Task<IActionResult> UpdateMarketplace(MarketplaceUpdateDto marketplace)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Marketplace) WHERE ID(n)=$marketplaceID
                                SET n.zone = $zone
                                SET n.restockCycle = $restockCycle
                                RETURN n";
                    var parameters = new 
                    { 
                        marketplaceID = marketplace.MarketplaceID,
                        zone = marketplace.Zone,
                        restockCycle= marketplace.RestockCycle
                         
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