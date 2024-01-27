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

        [HttpPost]
        public async Task<IActionResult> CreateMonsterBattle(MonsterBattleCreateDto dto)
        {
            try
            {
                var result = await _monsterBattleService.CreateAsync(dto);
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("Finalize")]
        public async Task<IActionResult> Finalize(MonsterBattleUpdateDto dto)
        {
            try
            {
                var result= await _monsterBattleService.Finalize(dto);
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
                var monsterBattle= await _monsterBattleService.GetAllAsync();
                return Ok(monsterBattle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int monsterBattleId)
        {
            try
            {
                var monsterBattles = await _monsterBattleService.GetOneAsync(monsterBattleId);
                return Ok(monsterBattles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int monsterBattleId)
        {
            try
            {
               var result= await _monsterBattleService.DeleteAsync(monsterBattleId);
               return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}