using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using ServiceStack.Redis;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonsterBattleController : ControllerBase
    {
        private readonly MonsterBattleService _monsterBattleService;
        private readonly PlayerService _playerService;

        public MonsterBattleController(MonsterBattleService monsterBattleService, PlayerService playerService)
        {
            _monsterBattleService = monsterBattleService;
            _playerService = playerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMonsterBattle(MonsterBattleCreateDto dto)
        {
            try
            {
                var result = await _monsterBattleService.CreateAsync(dto);
                return Ok();
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
                MonsterBattle monsterBattle = await _monsterBattleService.Finalize(dto);
                if (dto.LootItemsNames.Length > 0)
                {
                    foreach(string item in dto.LootItemsNames)
                    {
                        await _playerService.AddItemAsync(item, monsterBattle.Player.Name);
                    }
                }
                return Ok();
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
                var monsterBattles = await _monsterBattleService.GetAllAsync();
                return Ok(monsterBattles);
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
                var monsterBattle = await _monsterBattleService.GetOneAsync(monsterBattleId);
                return Ok(monsterBattle);
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
               var result = await _monsterBattleService.DeleteAsync(monsterBattleId);
               return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}