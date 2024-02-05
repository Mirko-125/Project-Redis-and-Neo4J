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
        private readonly ItemService _itemService;

        public PlayerController(PlayerService playerService, ItemService itemService)
        {
            _playerService = playerService;
            _itemService = itemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlayerDto player)
        {
            try
            {
                var result = await _playerService.CreateAsync(player);
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

        [HttpGet("GetOne")]
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

        [HttpPut("AddExperience")]
        public async Task<ActionResult> LevelUp(string playerName, int experience)
        {
            try
            {
                await _playerService.AddExperience(playerName, experience);
                var result = await _playerService.GetPlayerAsync(playerName);
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

        [HttpPut("EquipGear")]
        public async Task<ActionResult> EquipGear(string gearName, string playerName)
        {
            try
            {
                Player player = await _playerService.GetPlayerAsync(playerName);
                Item item = await _itemService.GetByNameAsync(gearName);
                if (item is Gear newGear)
                {
                    Console.WriteLine(newGear.Name, newGear.Slot);
                    foreach(Gear oldGear in player.Equipment.EquippedGear)
                    {
                        Console.WriteLine(oldGear.Name, oldGear.Slot);
                        if(oldGear.Slot == newGear.Slot)
                        {
                            await _playerService.UnequipGear(oldGear.Name, playerName);
                            await _playerService.AddItemAsync(oldGear.Name, playerName);
                        }
                    }
                    await _playerService.EquipGear(gearName, playerName);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePlayer(string playerName)
        {
            try
            {
                var result = await _playerService.DeleteAsync(playerName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}