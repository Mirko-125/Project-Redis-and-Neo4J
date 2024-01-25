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


        [HttpPost("AddMarketplace")]
        public async Task<IActionResult> AddMarketplace(MarketplaceCreateDto marketplace)
        {
            try
            {
                await _marketplaceService.AddAsync(marketplace);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("AddMarketplaceItems")]
        public async Task<IActionResult> AddMarketplaceItems(string zoneName, string itemName)
        {
            try
            {
                await _marketplaceService.AddItem(zoneName, itemName);
                return Ok();          
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllMarketplaces")]
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
   
        [HttpGet("GetMarketplace")]
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

        [HttpPut("UpdateMarketplace")]
        public async Task<IActionResult> UpdateMarketplace(MarketplaceUpdateDto marketplace)
        {
            try
            {
                var result = await _marketplaceService.UpdateMarketplaceAsync(marketplace);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     
        [HttpDelete("DeleteMarketplace")]
        public async Task<IActionResult> DeleteMarketplace(string zone)
        {
            try
            {    
                var result = await _marketplaceService.DeleteMarketplace(zone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

   
    }
}