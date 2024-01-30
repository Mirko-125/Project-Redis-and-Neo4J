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
    public class MarketplaceController : ControllerBase
    {
        private readonly MarketplaceService _marketplaceService;

        public MarketplaceController(MarketplaceService marketplaceService, RedisCache redisCache)
        {
            _marketplaceService = marketplaceService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateMarketplace(MarketplaceCreateDto dto)
        {
            try
            {
                await _marketplaceService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(string zoneName, string itemName)
        {
            try
            {
                var result = await _marketplaceService.AddItem(zoneName, itemName);
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
                var marketplaces = await _marketplaceService.GetAllAsync();
                return Ok(marketplaces);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
   
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(string zone)
        {
            try
            {    
                var market = await _marketplaceService.GetOneAsync(zone);
                return Ok(market);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMarketplace(MarketplaceUpdateDto marketplace)
        {
            try
            {
                var result = await _marketplaceService.UpdateAsync(marketplace);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     
        [HttpDelete]
        public async Task<IActionResult> DeleteMarketplace(string zone)
        {
            try
            {    
                var result = await _marketplaceService.DeleteAsync(zone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItem(string zone, string name)
        {
            try
            {    
                var result = await _marketplaceService.DeleteOneItemAsync(zone, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

   
    }
}