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

        [HttpPost("AddMonster")]
        public async Task<IActionResult> AddMonster(MonsterCreateDto monsterDto)
        {
            try
            {
               
                await _monsterService.AddAsync(monsterDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateMonster")]
        public async Task<IActionResult> UpdateMonster(MonsterUpdateDto monster)
        {
            try
            {
                var result = await _monsterService.UpdateMonsterAsync(monster);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllMonsters")]
        public async Task<IActionResult> GetAllMonsters()
        {
            try
            {
                var monsters = await _monsterService.GetMonstersAsync();
                return Ok(monsters);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetMonster")]
        public async Task<IActionResult> GetMonster(int monsterId)
        {
            try
            {
                var market = await _monsterService.GetMonsterAsync(monsterId);
                return Ok(market);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("DeleteMonster")]
        public async Task<IActionResult> RemoveMonster(int monsterId)
        {
            try
            {
                await _monsterService.DeleteMonster(monsterId);
                return Ok();
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}