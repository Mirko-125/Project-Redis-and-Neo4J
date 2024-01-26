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


        [HttpPost("AddGear")]
        public async Task<IActionResult> AddGear(GearCreateDto gear)
        {
            try
            {
                await _gearService.AddGearAsync(gear);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
           
        [HttpPut("UpdateGear")]
        public async Task<IActionResult> UpdateGear(GearUpdateDto gear)
        {
            try
            {
                var result = await _gearService.UpdateGearAsync(gear);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
    }
}