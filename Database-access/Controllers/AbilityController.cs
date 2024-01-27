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
    public class AbilityController : ControllerBase
    {
        private readonly AbilityService _abilityService;

        public AbilityController(AbilityService service)
        {
            _abilityService = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAbility(AbilityDTO ability)
        {
            try
            {
                await _abilityService.CreateAsync(ability);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AssignAbility")]
        public async Task<IActionResult> AssignAbility(string abilityName, string playerName)
        {
            try
            {
                await _abilityService.AssignAbilityAsync(abilityName, playerName);
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
                var abilities = await _abilityService.GetAllAsync();
                return Ok(abilities);
            }
        
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAbility(UpdateAbilityDto abilityDTO)
        {
            try
            {
                var ability = await _abilityService.UpdateAsync(abilityDTO);
                return Ok(ability);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteAbility(string abilityName)
        {
            try
            {
                await _abilityService.DeleteAsync(abilityName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}