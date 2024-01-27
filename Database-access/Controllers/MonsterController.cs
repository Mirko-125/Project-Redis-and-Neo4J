using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using Services;
using Neo4j.Driver.Preview.Mapping;


namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonsterController : ControllerBase
    {
        private readonly MonsterService _monsterService;
        
        public MonsterController(MonsterService monsterService)
        {
            _monsterService = monsterService;
           
        }

        [HttpPost]
        public async Task<IActionResult> Create(MonsterCreateDto dto)
        {
            try
            {
                await _monsterService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMonster(MonsterUpdateDto dto)
        {
            try
            {
                var result = await _monsterService.UpdateAsync(dto);
                return Ok(result);
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
                var monsters = await _monsterService.GetAllAsync();
                return Ok(monsters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int monsterId)
        {
            try
            {
                var monster = await _monsterService.GetMonsterAsync(monsterId);
                return Ok(monster);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveMonster(int monsterId)
        {
            try
            {
                var result = await _monsterService.DeleteAsync(monsterId);
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}