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


        [HttpPost("AddTrade")] 
        public async Task<IActionResult> AddTrade(TradeCreateDto trade)
        {
            try
            {
                await _tradeService.AddTradeAsync(trade);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpGet("GetAllTrades")]
        public async Task<IActionResult> GetAllTrades()
        {
            try
            {
                var trades= await _tradeService.GetAllTradesAsync();
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
        

        [HttpPut("FinalizeTrade")]
        public async Task<IActionResult> FinalizeTrade(TradeUpdateDto trade)
        {
            try
            {
                var result = await _tradeService.FinalizeTradeAsync(trade);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpDelete("DeleteTrade")]
        public async Task<IActionResult> DeleteTrade(int tradeID)
        {
            try
            {    
                var result = await _tradeService.DeleteTrade(tradeID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}