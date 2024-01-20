using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using ServiceStack.Redis;
using Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using ServiceStack.Text;
using System.Runtime.InteropServices;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketplaceController : ControllerBase
    {
        private readonly IDriver _driver;
        private readonly RedisCache cache;
        private string plularKey = "marketplaces";
        private string singularKey = "marketplace";

        public MarketplaceController(IDriver driver, RedisCache redisCache)
        {
            _driver = driver;
            cache = redisCache;
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
                            MATCH (market:Marketplace {zone: $zone})
                            MERGE (market)-[:HAS]->(n)

                        WITH market
                            MATCH (market)-[:HAS]->(i:Item) 
                                OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                            RETURN market as n, COLLECT({
                                item: i,
                                attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                            }) AS items";
                    var parameters = new {  
                        item = itemName,
                        zone = zoneName
                        };

                    var cursor = await session.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    await session.RunAsync(query, parameters);
                    var marketNode = record["n"].As<INode>();
                    var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
                    Marketplace market = new(marketNode, itemsNodeList);
                    await cache.SetDataAsync(singularKey + zoneName, market, 60);
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
                    var keyExists = await cache.CheckKeyAsync(plularKey);
                    if (keyExists)
                    {
                        var nesto = await cache.GetDataAsync<List<Marketplace>>(plularKey);
                        return Ok(nesto);          
                    }
                    var query = @"
                        MATCH (n:Marketplace)-[:HAS]->(i:Item) 
                            OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                        RETURN n, COLLECT({
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS items";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<Marketplace>();
                    
                    await cursor.ForEachAsync(record =>
                    {
                        var marketNode = record["n"].As<INode>();
                        var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
                        Marketplace market = new(marketNode, itemsNodeList);
                        resultList.Add(market);
                    });
                    foreach (var market in resultList)
                    {
                        await cache.SetDataAsync(singularKey + market.Zone, market, 25);
                    }
                    await cache.SetDataAsync(plularKey, resultList, 10);
                    return Ok(resultList);
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
                    var keyExists = await cache.CheckKeyAsync(singularKey + zone);
                    if (keyExists)
                    {
                        var nesto = await cache.GetDataAsync<List<Marketplace>>(singularKey + zone);
                        return Ok(nesto);          
                    }

                    var query = @"
                        MATCH (n:Marketplace)-[:HAS]->(i:Item) 
                            WHERE n.zone = $zone
                            OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                        RETURN n, COLLECT({
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS items";
                    var parameters = new { zone = zone };
                    var cursor = await session.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    await session.RunAsync(query, parameters);
                    var marketNode = record["n"].As<INode>();
                    var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
                    Marketplace market = new(marketNode, itemsNodeList);
                    await cache.SetDataAsync(singularKey + market.Zone, market, 60);

                    return Ok(market);
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
                    var cursor = await session.RunAsync(query, parameters);
                    await cache.DeleteAsync(singularKey + zone);
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