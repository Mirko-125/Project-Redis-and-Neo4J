using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using ServiceStack.Redis;
using Cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using ServiceStack.Text;
using System.Runtime.InteropServices;
using Services;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumableController : ControllerBase
    {
        private readonly ConsumableService _consumableService;

        public ConsumableController(ConsumableService consumableService)
        {
            _consumableService = consumableService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsumable(ConsumableCreateDto dto)
        {
            try
            {
                await _consumableService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
                   
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateConsumable(ConsumableUpdateDto dto)
        {
            try
            {
                var result = await _consumableService.UpdateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}