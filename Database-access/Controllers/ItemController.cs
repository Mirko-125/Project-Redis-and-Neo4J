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
                        var query = @"MATCH (n:Item) 
                                      OPTIONAL MATCH (n)-[:HAS]->(attributes:Attributes)
                                    RETURN  n, attributes";
                        var cursor = await tx.RunAsync(query);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var item = record["n"].As<INode>();
                            var connectedNodes = record["attributes"].As<INode>();
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
       
      [HttpGet("GetItemByName")]
        public async Task<IActionResult> GetItemByName(string name)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"MATCH (n:Item {name: $name})
                                      OPTIONAL MATCH (n)-[:HAS]->(attributes:Attributes)
                                    RETURN  n, attributes";
                                      
                        var parameters = new { name = name };
                        var cursor = await tx.RunAsync(query, parameters);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var item = record["n"].As<INode>();
                            var connectedNodes = record["attributes"].As<INode>();
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

        [HttpGet("GetItemByType")]
        public async Task<IActionResult> GetItemByType(string type)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"MATCH (n:Item {type: $type}) 
                                      OPTIONAL MATCH (n)-[:HAS]->(attributes:Attributes)
                                    RETURN  n, attributes";
                        var parameters = new { type = type };
        
                        var cursor = await tx.RunAsync(query, parameters);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var item = record["n"].As<INode>();
                            var connectedNodes = record["attributes"].As<INode>();
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
        
        [HttpDelete("RemoveItem")]
        public async Task<IActionResult> RemoveItem(string itemName)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:Item {name: $name}) 
                                  OPTIONAL MATCH (n)-[:HAS]->(attributes:Attributes)
                                DETACH DELETE n, attributes";
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