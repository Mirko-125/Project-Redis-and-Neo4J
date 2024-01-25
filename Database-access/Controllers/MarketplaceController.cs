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
        private readonly RedisCache cache;

        public MarketplaceController(MarketplaceService marketplaceService, RedisCache redisCache)
        {
            _marketplaceService = marketplaceService;
            cache = redisCache;
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
                Marketplace market = await _marketplaceService.AddItem(zoneName, itemName);
                await cache.SetDataAsync(_marketplaceService._key + zoneName, market, 60);
                return Ok();          
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetAllMarketplaces")]
        public async Task<IActionResult> GetAllMarketplaces()
        {
            try
            {
                var keyExists = await cache.CheckKeyAsync(_marketplaceService._pluralKey);
                if (keyExists)
                {
                    var nesto = await cache.GetDataAsync<List<Marketplace>>(_marketplaceService._key);
                    return Ok(nesto);          
                }
                var marketplaces = await _marketplaceService.GetMarketplacesAsync();
                foreach (var market in marketplaces)
                {
                    await cache.SetDataAsync(_marketplaceService._key + market.Zone, market, 100);
                }
                await cache.SetDataAsync(_marketplaceService._pluralKey, marketplaces, 100);
                return Ok(marketplaces);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
   
        [HttpGet("GetMarketplace")]
        public async Task<IActionResult> GetMarketplace(string zone)
        {
            try
            {
                string key = _marketplaceService._key + zone;
                var keyExists = await cache.CheckKeyAsync(key);
                if (keyExists)
                {
                    var cachedMarket = await cache.GetDataAsync<List<Marketplace>>(key);
                    return Ok(cachedMarket);          
                }
                    
                var market = _marketplaceService.GetMarketplaceAsync(zone);
                await cache.SetDataAsync(key, market, 60);
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
                return Ok(await cache.DeleteAsync(_marketplaceService._key + zone));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

   
    }
}