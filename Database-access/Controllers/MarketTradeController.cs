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
        public async Task<IActionResult> AddMarketTrade(MarketTradeDto dto)
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
                        playerItems=dto.PlayerItemNames,
                        marketItems=dto.MarketItemNames,
                        marketplace=dto.MarketplaceID
                    };

                    if (dto.PlayerItemNames.Length > 0){
                        var playerItemQuery =@"
                            MATCH (player:Player)-[:OWNS]->(inventory:Inventory)
                                WHERE ID(player) = $player

                            MATCH (playerItem: Item)
                                WHERE playerItem.name
                                    IN $playerItems 
                                    AND (inventory)-[:HAS]->(playerItem)

                            return playerItem"
                        ;
                        var playerItemResult = await session.RunAsync(playerItemQuery, parameters);
                        var items = await playerItemResult.ToListAsync();

                        Console.WriteLine("Added item");

                        if (items.Count != dto.PlayerItemNames.Length)
                            throw new Exception("The player doesn't own the items");
                    }

                    if (dto.MarketItemNames.Length > 0){
                        var marketItemQuery =@"
                            MATCH (market_:Marketplace)
                                WHERE ID(market_)=$marketplace

                            MATCH (marketItem: Item)
                                WHERE marketItem.name
                                    IN $marketItems
                                    AND (market_)-[:HAS]->(marketItem: Item)

                            return marketItem"
                        ;
                        var marketItemResult = await session.RunAsync(marketItemQuery, parameters);
                        var marketItems = await marketItemResult.ToListAsync();

                        if (marketItems.Count != dto.MarketItemNames.Length)
                            throw new Exception("The market doesn't own the items");
                    }

                    var query = @"
                        MATCH (player_:Player)
                            WHERE id(player_) = $player

                        MATCH (playerItem:Item)
                            WHERE playerItem.name IN $playerItems
                        WITH player_, collect(playerItem) as playerItemsList

                        MATCH (market_:Marketplace)
                            WHERE ID(market_) = $marketplace

                        MATCH (marketItem:Item)
                            WHERE marketItem.name IN $marketItems
                        WITH player_, playerItemsList, market_, collect(marketItem) as marketItemsList

                        CREATE (n:MarketTrade {
                            playerGold: $playerGold,
                            marketGold: $marketGold,
                            date: $date
                        })

                        WITH n, player_, market_, playerItemsList, marketItemsList

                        FOREACH (item IN playerItemsList |
                            CREATE (n)-[:PlayerItem]->(item)
                        )

                        FOREACH (item IN marketItemsList |
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

        [HttpGet("GetMarketTrades")]
        public async Task<IActionResult> GetMarketTrades(int playerID, int marketplaceID)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var result = await session.ExecuteReadAsync(async tx =>
                    {
                        var query = @"
                            MATCH (market:Marketplace)<-[:TradedAt]-(trade:MarketTrade)-[:TradingPlayer]->(player:Player)
                                WHERE id(player) = $player and id(market) = $marketplace
                            OPTIONAL MATCH (trade)-[rel]-(connectedNode)
                            RETURN trade, COLLECT(DISTINCT connectedNode) as connectedNodes
                        ";
                        
                        var parameters = new { 
                            player = playerID,
                            marketplace = marketplaceID
                        };
                        var cursor = await tx.RunAsync(query, parameters);
                        var resultList = new List<object>();

                        await cursor.ForEachAsync(record =>
                        {
                            var trade = record["trade"].As<INode>();
                            var connectedNodes = record["connectedNodes"].As<List<INode>>();
                            resultList.Add(new { Trade = trade, ConnectedNodes = connectedNodes });
                        });

                        return resultList;
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