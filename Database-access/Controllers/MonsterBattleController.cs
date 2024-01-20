using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;
using ServiceStack.Redis;

namespace Databaseaccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonsterBattleController : ControllerBase
    {
        private readonly IDriver _driver;

        private readonly RedisCache cache;
         private string pluralKey = "monsterBattles";
        private string singularKey = "monsterBattle";

        public MonsterBattleController(IDriver driver, RedisCache redisCache)
        {
            _driver = driver;
            cache = redisCache;
        }

        [HttpPost("AddMonsterBattle")]
        public async Task<IActionResult> AddMonsterBattle(MonsterBattleCreateDto mb)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
                     var parameters = new
                    {
                        startedAt=DateTime.Now,
                        endedAt="--",
                        isFinalized="false",
                        playeri=mb.PlayerId,
                        monsteri=mb.MonsterId
                    };
                    var query = @"
                        CREATE (n:MonsterBattle {
                            startedAt: $startedAt,
                            endedAt: $endedAt,
                            isFinalized: $isFinalized
                        }) 
                        WITH n
                        MATCH (monster:Monster) WHERE id(monster)=$monsteri
                        MATCH (player:Player) WHERE id(player)=$playeri
                        CREATE (monster)<-[:ATTACKED_MONSTER]-(n)-[:ATTACKING_PLAYER]->(player)
                        WITH n, monster, player

                        MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                                MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                        OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)
                            OPTIONAL MATCH (n)-[:LOOT]->(i:Item)
                                OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                        RETURN n, Id(n) AS monsterBattleId, player, monster, monsterAttributes, COLLECT(DISTINCT{
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS loot, COLLECT(DISTINCT{
                            item: pLoot,
                            attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                        }) AS possibleLoot "
                    ;
                    var cursor=await session.RunAsync(query, parameters);
                    var record =await cursor.SingleAsync();
                    var monsterBattleNode = record["n"].As<INode>();
                    var monsterBattleId = record["monsterBattleId"].As<string>();
                    var monsterNode = record["monster"].As<INode>();
                    var monsterAttributesNode = record["monsterAttributes"].As<INode>();
                    var playerNode = record["player"].As<INode>();
                    var lootNodeList = record["loot"].As<List<Dictionary<string, INode>>>();
                    var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
                    MonsterBattle monsterBattle= new(monsterBattleNode, monsterNode, monsterAttributesNode, playerNode,possibleLootNodeList,lootNodeList);
                    
                    string key=singularKey + monsterBattleId;
                    await cache.SetDataAsync(key, monsterBattle,1000);
                    return Ok(monsterBattle);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("FinalizeMonsterBattle")]
        public async Task<IActionResult> FinalizeMonsterBattle(MonsterBattleUpdateDto monsterBattleDto)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                     var parameters = new 
                    {   
                        monsterBattleId = monsterBattleDto.MonsterBattleId,
                        endedAt = DateTime.Now,
                        isFinalized= "true",
                        lootItems=monsterBattleDto.LootItemsNames
                    };
                    if (monsterBattleDto.LootItemsNames.Length > 0){
                        var lootQuery =@"
                            MATCH (monsterBattle:MonsterBattle) WHERE ID(monsterBattle)=$monsterBattleId
                            MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-(monsterBattle)
                            MATCH (lootItem: Item)
                                WHERE lootItem.name
                                    IN $lootItems
                                    AND (lootItem)<-[:POSSIBLE_LOOT]-(monster)
                            return lootItem"
                        ;
                        var lootItemResult = await session.RunAsync(lootQuery, parameters);
                        var lootItems = await lootItemResult.ToListAsync();

                        if (lootItems.Count != monsterBattleDto.LootItemsNames.Length)
                            throw new Exception("Some of the items don't exist or does not belongs to the Monster");
                    }
                    var query = @"
                        MATCH (monsterBattle:MonsterBattle) WHERE ID(monsterBattle)=$monsterBattleId
                            SET monsterBattle.endedAt= $endedAt
                            SET monsterBattle.isFinalized= $isFinalized
                        WITH monsterBattle

                        MATCH (lootItem: Item)
                            WHERE lootItem.name
                            IN $lootItems
                        WITH monsterBattle, collect(lootItem) as lootItemsList            
                                    
                        FOREACH (item IN lootItemsList |
                                CREATE (monsterBattle)-[:LOOT]->(item)
                        )
                        WITH monsterBattle
                        MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-(monsterBattle)-[:ATTACKING_PLAYER]->(player:Player)
                        MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                        OPTIONAL MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                            OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)
                        OPTIONAL MATCH (monsterBattle)-[:LOOT]->(i:Item)
                            OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                        RETURN monsterBattle, player, monster, monsterAttributes, COLLECT(DISTINCT{
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS loot, COLLECT(DISTINCT{
                            item: pLoot,
                            attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                        }) AS possibleLoot"     
                    ;
                    var cursor=await session.RunAsync(query, parameters);
                    var record =await cursor.SingleAsync();
                    var monsterBattleNode = record["monsterBattle"].As<INode>();
                    var monsterNode = record["monster"].As<INode>();
                    var monsterAttributesNode = record["monsterAttributes"].As<INode>();
                    var playerNode = record["player"].As<INode>();
                    var lootNodeList = record["loot"].As<List<Dictionary<string, INode>>>();
                    var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
                    MonsterBattle monsterBattlee= new(monsterBattleNode, monsterNode, monsterAttributesNode, playerNode,possibleLootNodeList,lootNodeList);
                    
                    string key=singularKey + monsterBattleDto.MonsterBattleId;
                    await cache.SetDataAsync(key, monsterBattlee, 10);
                    return Ok(monsterBattlee);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllMonsterBattles")]
        public async Task<IActionResult> GetAllMonsterBattles()
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    var keyExists = await cache.CheckKeyAsync(pluralKey);
                    if (keyExists)
                    {
                        var nesto = await cache.GetDataAsync<List<MonsterBattle>>(pluralKey);
                        return Ok(nesto);          
                    }
                    var query = @"
                        MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(player:Player)
                        MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                        OPTIONAL MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                            OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)
                        OPTIONAL MATCH (n)-[:LOOT]->(i:Item)
                            OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                        RETURN n, Id(n) AS monsterBattleId, player, monster, monsterAttributes, COLLECT(DISTINCT{
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS loot, COLLECT(DISTINCT{
                            item: pLoot,
                            attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                        }) AS possibleLoot";
                    var cursor = await session.RunAsync(query);
                    var resultList = new List<MonsterBattle>();
                    var ids=new List<string>();
                    await cursor.ForEachAsync(record =>
                    {
                        var monsterBattleNode = record["n"].As<INode>();
                        var monsterBattleId = record["monsterBattleId"].As<string>();
                        ids.Add(monsterBattleId);                        
                        var monsterNode = record["monster"].As<INode>();
                        var monsterAttributesNode = record["monsterAttributes"].As<INode>();
                        var playerNode = record["player"].As<INode>();
                        var lootNodeList = record["loot"].As<List<Dictionary<string, INode>>>();
                        var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
                        MonsterBattle monsterBattle= new(monsterBattleNode, monsterNode, monsterAttributesNode, playerNode,possibleLootNodeList,lootNodeList);
                        resultList.Add(monsterBattle);
                    });
                    int i=0;
                    foreach (var monsterBattleVar in resultList)
                    {   
                        await cache.SetDataAsync(singularKey + ids[i], monsterBattleVar, 250);
                        i++;
                    }
                    await cache.SetDataAsync(pluralKey, resultList, 100);
                    return Ok(resultList);
                }
             }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }       
        [HttpGet("GetMonsterBattle")]
        public async Task<IActionResult> GetMonsterBattle(int monsterBattleId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {
                    
                    var query =@"
                        MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(player:Player)
                            WHERE ID(n)=$idn
                        MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                        MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                            OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)
                        OPTIONAL MATCH (n)-[:LOOT]->(i:Item)
                            OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                        RETURN n, player, monster, monsterAttributes, COLLECT(DISTINCT{
                            item: i,
                            attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                        }) AS loot, COLLECT(DISTINCT{
                            item: pLoot,
                            attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                        }) AS possibleLoot";
                    var parameters = new { idn = monsterBattleId };
                    var cursor = await session.RunAsync(query,parameters);
                    var resultList = new List<MonsterBattle>();
                    await cursor.ForEachAsync(record =>
                    {
                        var monsterBattleNode = record["n"].As<INode>();
                        var monsterNode = record["monster"].As<INode>();
                        var monsterAttributesNode = record["monsterAttributes"].As<INode>();
                        var playerNode = record["player"].As<INode>();
                        var lootNodeList = record["loot"].As<List<Dictionary<string, INode>>>();
                        var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
                        MonsterBattle monsterBattle= new(monsterBattleNode, monsterNode, monsterAttributesNode, playerNode,possibleLootNodeList,lootNodeList);
                        resultList.Add(monsterBattle);
                    });
                    return Ok(resultList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
           
        [HttpDelete("DeleteMonsterBattle")]
        public async Task<IActionResult> RemoveMonsterBattle(int monsterBattleId)
        {
            try
            {
                using (var session = _driver.AsyncSession())
                {   
                    var query = @"MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(n2:Player) WHERE id(n)=$mbId DETACH DELETE n";
                    var parameters = new { mbId = monsterBattleId};
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