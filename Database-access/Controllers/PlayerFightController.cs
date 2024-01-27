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

        public PlayerFightController(PlayerFightService playerFightService)
        {
            _playerFightService = playerFightService;
        }

        [HttpPost("AddPlayerFight")]
        public async Task<IActionResult> AddPlayerFight(PlayerFightCreateDto playerFight)
        {
            try
            {
                var result = await _playerFightService.AddAsync(playerFight);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("UpdatePlayerFight")]
        public async Task<IActionResult> UpdatePlayerFight(PlayerFightUpdateDto playerFight)
        {
            try
            {
                var result = await _playerFightService.UpdatePlayerFightAsync(playerFight);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllPlayerFights")]
        public async Task<IActionResult> GetAllPlayerFights()
        {
            try
            {
                var playerFights = await _playerFightService.GetPlayerFightsAsync();
                return Ok(playerFights);
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetPlayerFight")]
        public async Task<IActionResult> GetPlayerFight(int playerFightId)
        {
            try
            {
                var playerFight = await _playerFightService.GetPlayerFightAsync(playerFightId);
                return Ok(playerFight);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
              
        [HttpDelete("DeletePlayerFight")]
        public async Task<IActionResult> RemovePlayerFight(int playerFightId)
        {
            try
            {
                var result = await _playerFightService.DeletePlayerFight(playerFightId);
                return Ok(result);   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}