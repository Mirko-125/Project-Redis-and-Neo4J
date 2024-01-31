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
        private readonly PlayerService _playerService;

        public TradeController(TradeService tradeService, RedisCache redisCache, PlayerService playerService)
        {
            _tradeService = tradeService;
            _playerService = playerService;
        }

        [HttpPost] 
        public async Task<IActionResult> CreateAsync(TradeCreateDto dto)
        {
            try
            {
                Trade trade = await _tradeService.CreateAsync(dto);
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
        public async Task<IActionResult> GetPlayerTrades(string playerName)
        {
            try
            {    
                var trades = await _tradeService.GetPlayerTradesAsync(playerName);
                return Ok(trades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPut("Finalize")]
        public async Task<IActionResult> Finalize(int tradeID)
        {
            try
            {
                Trade trade = await _tradeService.FinalizeAsync(tradeID);
                
                if (trade.RequesterGold < 0 || trade.ReceiverGold < 0)
                {
                    throw new Exception ("You can't trade negative gold!");
                }
                
                if (trade.RequesterItems.Count > 0)
                {
                    foreach(Item item in trade.RequesterItems)
                    {
                        await _playerService.AddItemAsync(item.Name, trade.Receiver.Name);
                        await _playerService.RemoveItem(item.Name, trade.Requester.Name);
                    }
                }
                
                if (trade.ReceiverItems.Count > 0)
                {
                    foreach(Item item in trade.ReceiverItems)
                    {
                        await _playerService.AddItemAsync(item.Name, trade.Requester.Name);
                        await _playerService.RemoveItem(item.Name, trade.Receiver.Name);
                    }
                }

                int reqGold = trade.ReceiverGold - trade.RequesterGold;
                int recGold =  trade.RequesterGold - trade.ReceiverGold;
                await _playerService.AddGold(trade.Receiver.Name, recGold);
                await _playerService.AddGold(trade.Requester.Name, reqGold);
                trade.Receiver.Gold += recGold;
                trade.Requester.Gold += reqGold;
                return Ok(trade);
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