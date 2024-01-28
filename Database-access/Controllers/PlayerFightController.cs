using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerFightController : ControllerBase
    {
        private readonly PlayerFightService _playerFightService;
        private readonly PlayerService _playerService;

        public PlayerFightController(PlayerFightService playerFightService, PlayerService playerService)
        {
            _playerFightService = playerFightService;
            _playerService = playerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFight(PlayerFightCreateDto playerFight)
        {
            try
            {
                await _playerFightService.CreateAsync(playerFight);
                await _playerService.AddHonor(playerFight.Winner, playerFight.Honor);
                await _playerService.AddExperience(playerFight.Winner, playerFight.Experience);
                var result = await _playerService.GetPlayerAsync(playerFight.Winner);
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
                var playerFights = await _playerFightService.GetAllAsync();
                return Ok(playerFights);
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int playerFightId)
        {
            try
            {
                var playerFight = await _playerFightService.GetOneAsync(playerFightId);
                return Ok(playerFight);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdatePlayerFight(PlayerFightUpdateDto playerFight)
        {
            try
            {
                var result = await _playerFightService.UpdateAsync(playerFight);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
              
        [HttpDelete]
        public async Task<IActionResult> RemovePlayerFight(int playerFightId)
        {
            try
            {
                var result = await _playerFightService.DeleteAsync(playerFightId);
                return Ok();   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}