using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerFightController : ControllerBase
    {
        private readonly IDriver _driver;

        public PlayerFightController(IDriver driver)
        {
            _driver = driver;
        }
        [HttpPost("AddPlayerFight")]
        public async Task<IActionResult> AddPlayerFight(PlayerFight plf, int player1ID,int player2ID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"
                        CREATE (n:PlayerFight {
                            winner: $winner,
                            experience: $experience,
                            honor: $honor
                        })
                        WITH n
                        MATCH (n1:Player) WHERE id(n1)=$playeri1
                        MATCH (n2:Player) WHERE id(n2)=$playeri2
                        CREATE (n1)-[:PARTICIPATING_IN]->(n)<-[:PARTICIPATING_IN]-(n2) ";

                    var parameters = new
                    {
                        winner=plf.Winner,
                        experience=plf.Experience,
                        honor=plf.Honor,
                        playeri1=player1ID,
                        playeri2=player2ID
                    };
                    var result=await session.RunAsync(query, parameters);
                   /* var idn=await result.SingleAsync();
                    var nodeId = idn["nodeId"].As<long>();
                    var query1 = @"
                        MATCH (n1:Player) WHERE id(n1)=$playeri1
                        MATCH (n2:Player) WHERE id(n2)=$playeri2
                        MATCH (n:PlayerFight) WHERE id(n)=$idn
                        CREATE (n1)-[:PARTICIPATING_IN]->(n)<-[:PARTICIPATING_IN]-(n2) ";
                    var parameters1 = new
                    {
                       playeri1=player1ID,
                       playeri2=player2ID,
                       idn=nodeId
                    };
                    await session.RunAsync(query1, parameters1);*/
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllPlayerFights")]
        public async Task<IActionResult> GetAllPlayerFights()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n:PlayerFight) RETURN n";
                        var cursor = await tx.RunAsync(query);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
             }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetPlayerFightsBetweenTwoPlayers")]
        public async Task<IActionResult> GetPlayerFightsBetweenTwoPlayers(int player1ID,int player2ID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n1:Player)-[:PARTICIPATING_IN]->(n:PlayerFight)<-[:PARTICIPATING_IN]-(n2:Player) WHERE id(n1)=$playeri1 and id(n2)=$playeri2 RETURN n";
                        var parameters = new { playeri1 = player1ID,
                        playeri2=player2ID };
                        var cursor = await tx.RunAsync(query,parameters);
                        var nodes = new List<INode>();

                        await cursor.ForEachAsync(record =>
                        {
                            var node = record["n"].As<INode>();
                            nodes.Add(node);
                        });

                        return nodes;
                    });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       [HttpDelete("DeletePlayerFight")]
        public async Task<IActionResult> RemovePlayerFight(int player1ID,int player2ID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   //nema error i ako cvor koji zelimo da obrisemo ne postoji
                    var query = @"MATCH (n1:Player)-[:PARTICIPATING_IN]->(n)<-[:PARTICIPATING_IN]-(n2:Player) WHERE id(n1)=$playeri1 and id(n2)=$playeri2 DETACH DELETE n";
                    var parameters = new { playeri1 = player1ID,
                    playeri2=player2ID};
                    await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}