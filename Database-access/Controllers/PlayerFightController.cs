using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using System.Reflection.Metadata;

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
        public async Task<IActionResult> AddPlayerFight(PlayerFightCreateDto playerFight)
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
                        CREATE (n1)<-[:PARTICIPATING_PLAYERS]-(n)-[:PARTICIPATING_PLAYERS]->(n2) ";

                    var parameters = new
                    {
                        winner=playerFight.Winner,
                        experience=playerFight.Experience,
                        honor=playerFight.Honor,
                        playeri1=playerFight.Player1Id,
                        playeri2=playerFight.Player2Id
                    };
                    var result=await session.RunAsync(query, parameters);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("UpdatePlayerFight")]
        public async Task<IActionResult> UpdatePlayerFight(PlayerFightUpdateDto playerFight)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var query = @"MATCH (n:PlayerFight) WHERE Id(n)=$playerFightid
                                SET n.winner= $winner
                                SET n.experience= $experience
                                SET n.honor= $honor
                                RETURN n";
                    var parameters = new 
                    { 
                        playerFightid = playerFight.PlayerFightId,
                        winner=playerFight.Winner,
                        experience= playerFight.Experience,
                        honor=playerFight.Honor 
                    };
                    await session.RunAsync(query, parameters);
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
                    var query = @"MATCH (playerFight:PlayerFight)-[:PARTICIPATING_PLAYERS]->(p:Player) 
                                    RETURN playerFight, COLLECT(p) as players";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<PlayerFight>();

                    await cursor.ForEachAsync(record =>
                    {
                        var playerFightNode = record["playerFight"].As<INode>();
                        var players = record["players"].As<List<INode>>();
                        var player1Node = players[0];
                        var player2Node = players[1];
                        PlayerFight playerFight= new(playerFightNode, player1Node, player2Node);
                        resultList.Add(playerFight);
                    });

                    return Ok(resultList);
                }
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("GetPlayerFight")]
        public async Task<IActionResult> GetPlayerFight(int playerFightId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var parameters = new { plFId = playerFightId };
                    var query = @"MATCH (playerFight:PlayerFight)-[:PARTICIPATING_PLAYERS]->(p:Player)
                                    WHERE id(playerFight)= $plFId
                                RETURN playerFight, COLLECT(p) as players";
                    var cursor = await session.RunAsync(query,parameters);
                    var record=await cursor.SingleAsync();
                    var playerFightNode = record["playerFight"].As<INode>();
                    var players = record["players"].As<List<INode>>();
                    var player1Node = players[0];
                    var player2Node = players[1];
                    PlayerFight playerFight= new(playerFightNode, player1Node, player2Node);              

                    return Ok(playerFight);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
              
        [HttpDelete("DeletePlayerFight")]
        public async Task<IActionResult> RemovePlayerFight(int plFightId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
                    var query = @"MATCH (n:PlayerFight) WHERE id(n)=$pfId DETACH DELETE n";
                    var parameters = new { pfId = plFightId };
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