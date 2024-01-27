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
    public class TradeController : ControllerBase
    {
        private readonly TradeService _tradeService;

        public TradeController(TradeService tradeService, RedisCache redisCache)
        {
            _tradeService = tradeService;
        }

        [HttpPost] 
        public async Task<IActionResult> CreateAsync(TradeCreateDto dto)
        {
            try
            {
                await _tradeService.CreateAsync(dto);
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
                var trades= await _tradeService.GetAllAsync();
                return Ok(trades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpGet("GetPlayerTrades")]
        public async Task<IActionResult> GetPlayerTrades(int playerid)
        {
            try
            {    
                var trades = await _tradeService.GetPlayerTradesAsync(playerid);
                return Ok(trades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPut("Finalize")]
        public async Task<IActionResult> Finalize(TradeUpdateDto trade)
        {
            try
            {
                var result = await _tradeService.FinalizeAsync(trade);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpDelete]
        public async Task<IActionResult> DeleteTrade(int tradeID)
        {
            try
            {    
                var result = await _tradeService.DeleteAsync(tradeID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}