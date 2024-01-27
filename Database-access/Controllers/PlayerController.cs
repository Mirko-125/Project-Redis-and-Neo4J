using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using System.Reflection.Metadata.Ecma335;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayerController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PlayerDto player)
        {
            try
            {
                var result = await _playerService.AddPlayerAsync(player);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> AllPlayers()
        {
            try
            {
                var players = await _playerService.GetAllAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayer(string name)
        {
            try
            {
                var player = await _playerService.GetPlayerAsync(name);
                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("LevelUp")]
        public async Task<ActionResult> LevelUp(string playerName)
        {
            try
            {
                var result = await _playerService.LevelUpAsync(playerName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("AddItem")]
        public async Task<IActionResult> AddItem(string itemName, string playerName)
        {
            try
            {
                var result = await _playerService.AddItemAsync(itemName, playerName);
                return Ok(result);   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> RemovePlayer(string playerName)
        {
            try
            {
                var result = await _playerService.RemovePlayerAsync(playerName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}