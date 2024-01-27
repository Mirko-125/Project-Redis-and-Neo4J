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

        [HttpPost]
        public async Task<IActionResult> AddNPC(NPCCreateDto dto)
        {
            try
            {
                var result = await _npcService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateNPC(NPCUpdateDto dto)
        {
            try
            {
                var result = await _npcService.UpdateAsync(dto);
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
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var npcs = await _npcService.GetAllAsync();
                return Ok(npcs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int npcId)
        {
            try
            {
                var npc = await _npcService.GetOneAsync(npcId);
                return Ok(npc);     
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int npcId)
        {
            try
            {
                var result = await _npcService.DeleteAsync(npcId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}