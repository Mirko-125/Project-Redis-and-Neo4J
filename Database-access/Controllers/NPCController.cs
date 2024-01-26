using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NPCController : ControllerBase
    {
        private readonly NPCService _npcService;
        
        public NPCController( NPCService npcService)
        {
            _npcService = npcService;       
        }

        [HttpPost("AddNPC")]
        public async Task<IActionResult> AddNPC(NPCCreateDto npc)
        {
            try
            {
                var result = await _npcService.AddAsync(npc);
                return Ok(result);
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
                var result = await _npcService.UpdateNPCAsync(npc);
                return Ok(result);
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
                var npc = await _npcService.Interaction(playerId,npcId);
                return Ok(npc);
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
                var npcs = await _npcService.GetNPCsAsync();
                return Ok(npcs);
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
                var npc = await _npcService.GetNPCAsync(npcId);
                return Ok(npc);     
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
                var result = await _npcService.DeleteNPC(npcId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}