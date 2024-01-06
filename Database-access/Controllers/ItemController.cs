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
    public class ItemController : ControllerBase
    {
        private readonly IDriver _driver;

        public ItemController(IDriver driver)
        {
            _driver = driver;
        }


        [HttpGet("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Item) RETURN n";
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
       
      [HttpGet("GetItemByName")]
        public async Task<IActionResult> GetItemByName(String name)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Item {name: $name}) RETURN n";
                        var parameters = new { name = name };
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

        [HttpGet("GetItemByType")]
        public async Task<IActionResult> GetItemByType(String type)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:Item {type: $type}) RETURN n";
                        var parameters = new { type = type };
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
        //   [HttpPost]
        //  public async Task<IActionResult> AddItem(Item item)
        // {
        //     try
        //     {
        //         using (var session = _driver.AsyncSession())
        //         {
        //             var query = @"
        //                 CREATE (n:Item {
        //                     name: $name,
        //                     weight: $weight,
        //                     type: $type,
        //                     dimensions: $dimensions,
        //                     value: $value
        //                 })";

        //             var parameters = new
        //             {
        //                 name = item.Name,
        //                 weight = item.Weight,
        //                 type = item.Type,
        //                 dimensions = item.Dimensions,
        //                 value = item.Value
        //             };
        //             await session.RunAsync(query, parameters);
        //             return Ok();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ex.Message);
        //     }
        // }

        
        [HttpDelete("RemoveItem")]
        public async Task<IActionResult> RemoveItem(String itemName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Item {name: $name}) DELETE n";
                    var parameters = new { name = itemName };
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