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
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService, RedisCache redisCache)
        {
            _itemService = itemService;
        }

        [HttpGet("GetAllItems")]

         public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var items = await _itemService.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        [HttpGet("GetItemByName")]
        public async Task<IActionResult> GetItemByName(string name)
        {
            try
            {    
                var items = await _itemService.GetItemByNameAsync(name);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetItemsByType")]
        public async Task<IActionResult> GetItemsByType(string type)
        {
            try
            {    
                var items = await _itemService.GetItemsByTypeAsync(type);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
        
        [HttpDelete("RemoveItem")]
        public async Task<IActionResult> DeleteItem(string name)
        {
            try
            {    
                var result = await _itemService.DeleteItem(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
           
    }
}