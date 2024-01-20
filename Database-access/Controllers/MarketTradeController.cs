using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databaseaccess.Models;
using Cache;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketTradeController : ControllerBase
    {
        private readonly IDriver _driver;
        private readonly RedisCache cache;

        public MarketTradeController(IDriver driver, RedisCache redisCache)
        {
            _driver = driver;
            cache = redisCache;
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
                            MERGE (n)-[:PlayerItem]->(item)
                        )

                        FOREACH (item IN marketItemsList |
                            MERGE (n)-[:MarketItem]->(item)
                        )
                        
                        MERGE (n)-[:TradingPlayer]->(player_)
                        MERGE (n)-[:TradedAt]->(market_)
                        
                        WITH market_ as market 
                            MATCH (market)-[:HAS]->(i:Item) 
                                OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                            RETURN market as n, COLLECT({
                                item: i,
                                attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                            }) AS items";

                    var cursor = await session.RunAsync(query, parameters);
                    var record = await cursor.SingleAsync();
                    var marketNode = record["n"].As<INode>();
                    var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
                    Marketplace market = new(marketNode, itemsNodeList);
                    await cache.SetDataAsync("marketplace" + market.Zone, market, 60);
                    return Ok();
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
                            MATCH (market:Marketplace)<-[:TradedAt]-(n:MarketTrade)-[:TradingPlayer]->(player:Player)
                                WHERE id(player)=$player and id(market)=$marketplace
                            RETURN n";
                        
                        var parameters = new { 
                            player = playerID,
                            marketplace=marketplaceID
                        };
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