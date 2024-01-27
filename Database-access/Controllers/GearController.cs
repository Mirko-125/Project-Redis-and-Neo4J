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
    public class GearController : ControllerBase
    {
        private readonly GearService _gearService;

        public GearController(GearService gearService)
        {
            _gearService = gearService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGear(GearCreateDto dto)
        {
            try
            {
                await _gearService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
           
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGear(GearUpdateDto dto)
        {
            try
            {
                var result = await _gearService.UpdateGearAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}