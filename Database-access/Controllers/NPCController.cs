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
    public class NPCController : ControllerBase
    {
        private readonly IDriver _driver;

        public NPCController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost("AddNPC")]
        public async Task<IActionResult> AddNPC(NPC npc)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:NPC {
                            name: $name,
                            affinity: $affinity,
                            imageUrl: $imageUrl,
                            zone: $zone,
                            mood: $mood
                        })";

                    var parameters = new
                    {
                        name = npc.Name,
                        affinity = npc.Affinity,
                        imageUrl= npc.ImageURL,
                        zone = npc.Zone,                        
                        mood=npc.Mood
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
        [HttpPut("Interaction")]
        public async Task<IActionResult> Interaction(int playerId, int npcId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                        var query = @"MATCH (n1:NPC) WHERE id(n1)=$npId 
                                    MATCH (n2:Player) WHERE id(n2)=$plId
                                    MERGE (n2)-[rel:INTERACTS_WITH]->(n1)
                                    SET rel.property_key = COALESCE(rel.property_key, 0) + 1
                                    RETURN rel.property_key AS incrementedProperty;";
                        
                        var parameters = new {npId=npcId,
                                                plId = playerId};
                        var cursor = await session.RunAsync(query,parameters);
                        var n = await cursor.SingleAsync();
                        var seq = n["incrementedProperty"].As<int>();
                    
                   return Ok(seq);
                }
             }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetNPCs")]
        public async Task<IActionResult> GetAllNPCs()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:NPC) RETURN n";
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
        [HttpGet("GetNPC")]
        public async Task<IActionResult> GetNPC(int npcId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:NPC) WHERE id(n)=$id RETURN n";
                        var parameters = new { id= npcId };
                        var cursor = await tx.RunAsync(query,parameters);
                        var n = await cursor.SingleAsync();
                        var node = n["n"].As<INode>();
                        return node;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("DeleteNPC")]
        public async Task<IActionResult> RemoveNPC(int npcId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n:NPC) WHERE id(n)=$id DETACH DELETE n";
                    var parameters = new { id = npcId };
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