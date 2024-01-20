using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;
using ServiceStack.Redis;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NPCController : ControllerBase
    {
        private readonly IDriver _driver;

        private readonly RedisCache cache;
        private string pluralKey = "NPCs";
        private string singularKey = "NPC";

        public NPCController(IDriver driver, RedisCache redisCache)
        {
            _driver = driver;
            cache = redisCache;
        }

        [HttpPost("AddNPC")]
        public async Task<IActionResult> AddNPC(NPCCreateDto npc)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:NPC {
                            name: $name,
                            affinity: $affinity,
                            imageURL: $imageUrl,
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
        [HttpPut("UpdateNPC")]
        public async Task<IActionResult> UpdateNPC(NPCUpdateDto npc)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:NPC) WHERE ID(n)=$npcId
                                SET n.name= $name
                                SET n.affinity= $affinity
                                SET n.imageURL= $imageUrl
                                SET n.zone= $zone
                                SET n.mood= $mood
                                RETURN n";
                    var parameters = new 
                    { 
                        npcId = npc.NPCId,
                        name=npc.Name,
                        affinity=npc.Affinity,
                        imageUrl=npc.ImageURL,
                        zone=npc.Zone,
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
                    string key = singularKey + npcId;
                    var keyExists = await cache.CheckKeyAsync(key);
                    if (keyExists)
                    {
                        var nesto = await cache.GetDataAsync<NPC>(key);
                        return Ok(nesto);          
                    }
                    var query = @"
                        MATCH (n1:NPC) WHERE id(n1)=$npId 
                        MATCH (n2:Player) WHERE id(n2)=$plId
                        MERGE (n2)-[rel:INTERACTS_WITH]->(n1)
                        SET rel.property_key = COALESCE(rel.property_key, 0) + 1
                        RETURN rel.property_key AS incrementedProperty,n1"
                    ;
                    var parameters = new 
                    {   npId=npcId,
                        plId = playerId
                    };
                    var cursor = await session.RunAsync(query,parameters);
                    var n = await cursor.SingleAsync();
                    var seq = n["incrementedProperty"].As<int>();
                    var node = n["n1"].As<INode>();

                    NPC novi=new(node);
                    
                    await cache.SetDataAsync(singularKey + npcId, novi, 25);
                    return Ok(novi);
                }
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllNPCs")]
        public async Task<IActionResult> GetAllNPCs()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = "MATCH (n:NPC) RETURN n";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<NPC>();

                    await cursor.ForEachAsync(record =>
                    {
                        var npcNode = record["n"].As<INode>();
                        NPC nps=new(npcNode);
                        resultList.Add(nps);
                    });

                    return Ok(resultList);
                }
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetNPC")]
        public async Task<IActionResult> GetNPC(int npcId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    string key = singularKey + npcId;
                    var keyExists = await cache.CheckKeyAsync(key);
                    if (keyExists)
                    {
                        var nesto = await cache.GetDataAsync<NPC>(key);
                        return Ok(nesto);          
                    }
                    var query = "MATCH (n:NPC) WHERE id(n)=$id RETURN n";
                    var parameters = new { id= npcId };
                    var cursor = await session.RunAsync(query,parameters);
                    var n = await cursor.SingleAsync();
                    var node = n["n"].As<INode>();
                    NPC npc=new(node);
                    return Ok(npc);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteNPC")]
        public async Task<IActionResult> RemoveNPC(int npcId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
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