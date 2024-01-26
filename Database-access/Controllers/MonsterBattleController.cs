using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;
using ServiceStack.Redis;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonsterBattleController : ControllerBase
    {
        private readonly MonsterBattleService _monsterBattleService;

        public MonsterBattleController(MonsterBattleService monsterBattleService, RedisCache redisCache)
        {
            _monsterBattleService= monsterBattleService;
        }

        [HttpPost("AddMonsterBattle")]
        public async Task<IActionResult> AddMonsterBattle(MonsterBattleCreateDto mb)
        {
            try
            {
                var result = await _monsterBattleService.AddAsync(mb);
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("FinalizeMonsterBattle")]
        public async Task<IActionResult> FinalizeMonsterBattle(MonsterBattleUpdateDto monsterBattleDto)
        {
            try
            {
                var result= await _monsterBattleService.UpdateMonsterBattleAsync(monsterBattleDto);
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllMonsterBattles")]
        public async Task<IActionResult> GetAllMonsterBattles()
        {
            try
            {
                var result= await _monsterBattleService.GetMonsterBattlesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        [HttpGet("GetMonsterBattle")]
        public async Task<IActionResult> GetMonsterBattle(int monsterBattleId)
        {
            try
            {
                var result = await _monsterBattleService.GetMonsterBattleAsync(monsterBattleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteMonsterBattle")]
        public async Task<IActionResult> RemoveMonsterBattle(int monsterBattleId)
        {
            try
            {
               var result= await _monsterBattleService.DeleteMonsterBattle(monsterBattleId);
               return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}