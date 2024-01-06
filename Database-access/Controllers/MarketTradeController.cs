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
    public class MarketTradeController : ControllerBase
    {
        private readonly IDriver _driver;

        public MarketTradeController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost]
        public async Task<IActionResult> AddTradeOfPlayerAndMarketplace(MarketTradeDto dto)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {

                    var parameters = new
                    {
                        playerGold=dto.PlayerGold,
                        marketGold=dto.MarketGold,
                        date=dto.Date,
                        player=dto.PlayerID,
                        playerItems=dto.PlayerItemsIDs,
                        marketItems=dto.MarketItemsIDs,
                        marketplace=dto.MarketplaceID
                    };

                    if (dto.PlayerItemsIDs.Length > 0){
                        var playerItemQuery =@"
                            MATCH (player:Player)-[:OWNS]->(inventory:Inventory)
                                WHERE ID(player) = $player

                            MATCH (playerItem: Item)
                                WHERE ID(playerItem)
                                    IN $playerItems 
                                    AND (inventory)-[:HAS]->(playerItem)

                            return playerItem"
                        ;
                        var playerItemResult = await session.RunAsync(playerItemQuery, parameters);
                        var items = await playerItemResult.ToListAsync();

                        Console.WriteLine("Added item");

                        if (items.Count != dto.PlayerItemsIDs.Length)
                            throw new Exception("The player doesn't own the items");
                    }

                    if (dto.MarketItemsIDs.Length > 0){
                        var marketItemQuery =@"
                            MATCH (market_:Marketplace)
                                WHERE ID(market_)=$marketplace

                            MATCH (marketItem: Item)
                                WHERE ID(marketItem)
                                    IN $marketItems
                                    AND (market_)-[:HAS]->(marketItem: Item)

                            return marketItem"
                        ;
                        var marketItemResult = await session.RunAsync(marketItemQuery, parameters);
                        var marketItems = await marketItemResult.ToListAsync();

                        if (marketItems.Count != dto.MarketItemsIDs.Length)
                            throw new Exception("The market doesn't own the items");
                    }

                    var query = @"
                        MATCH (player_:Player)
                            WHERE id(player_) = $player

                        MATCH (playerItem:Item)
                            WHERE ID(playerItem) IN $playerItems

                        MATCH (market_:Marketplace)
                            WHERE ID(market_) = $marketplace

                        MATCH (marketItem:Item)
                            WHERE ID(marketItem) IN $marketItems

                        CREATE (n:MarketTrade {
                            playerGold: $playerGold,
                            marketGold: $marketGold,
                            date: $date
                        })

                        WITH n, player_, playerItem, market_, marketItem

                        FOREACH (item IN [playerItem] |
                            CREATE (n)-[:PlayerItem]->(item)
                        )

                        FOREACH (item IN [marketItem] |
                            CREATE (n)-[:MarketItem]->(item)
                        )
                        
                        CREATE (n)-[:TradingPlayer]->(player_)
                        CREATE (n)-[:TradedAt]->(market_)
                        
                        return n"
                    ;

                    
                    var result = await session.RunAsync(query, parameters);
                    return Ok(result.ToString());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTradesBetweenPlayerAndMarketplace")]
        public async Task<IActionResult> GetTradesBetweenPlayerAndMarketplace(int playerID,int marketplaceID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = "MATCH (n1:Player)-[:RECEIVER]->(n:Trade)<-[:REQUESTER]-(n2:Marketplace) WHERE id(n1)=$player and id(n2)=$marketplace RETURN n";
                        
                        var parameters = new { player = playerID,
                        marketplace=marketplaceID};
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
        
    }
}