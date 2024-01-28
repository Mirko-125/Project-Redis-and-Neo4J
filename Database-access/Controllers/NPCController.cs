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
                return Ok();
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Interaction")]
        public async Task<IActionResult> Interaction(string playerName, string npcName)
        {
            try
            {
                var npc = await _npcService.Interaction(playerName, npcName);
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
        public async Task<IActionResult> GetOne(string npcName)
        {
            try
            {
                var npc = await _npcService.GetOneAsync(npcName);
                return Ok(npc);     
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string npcName)
        {
            try
            {
                var result = await _npcService.DeleteAsync(npcName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}