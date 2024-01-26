using Cache;
using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class MonsterBattleService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "MonsterBattle";
        public readonly string _pluralKey = "monsterBattles";
        public readonly string _key = "monsterBattle";

        public static MonsterBattle BuildMonsterBattle(IRecord record)
        {
            var monsterBattleNode = record["n"].As<INode>();
            var lootNodeList = record["items"].As<List<Dictionary<string, INode>>>();
            var monsterBattleId = record["monsterBattleId"].As<string>();   
            var playerNode = record["player"].As<INode>();
            var monsterNode = record["monster"].As<INode>();
            var monsterAttributesNode = record["monsterAttributes"].As<INode>();
            var possibleLootNodeList = record["possibleLoot"].As<List<Dictionary<string, INode>>>();
            return new(monsterBattleNode, monsterNode, monsterAttributesNode, playerNode, possibleLootNodeList, lootNodeList);
        }

        public MonsterBattleService(IDriver driver, RedisCache cache)
        {
            _driver = driver;
            _cache=cache;
        }

        public async Task<MonsterBattle> AddAsync(MonsterBattleCreateDto mb)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                startedAt=DateTime.Now,
                endedAt="--",
                isFinalized="false",
                playeri=mb.PlayerId,
                monsteri=mb.MonsterId
            };
            string query = $@"
                CREATE ({_key}:{type} {{
                    startedAt: $startedAt,
                    endedAt: $endedAt,
                    isFinalized: $isFinalized
                }}) 
                WITH {_key}
                MATCH (monster:Monster) WHERE id(monster)=$monsteri
                MATCH (player:Player) WHERE id(player)=$playeri
                CREATE (monster)<-[:ATTACKED_MONSTER]-({_key})-[:ATTACKING_PLAYER]->(player)
                WITH {_key}, monster, player"
            ;
            query += ReturnMonsterBattleWithAllItems(type, _key, "LOOT");
            //Console.WriteLine(query);
            var cursor=await session.RunAsync(query, parameters);
            var record=await cursor.SingleAsync();
            var monsterBattleId=record["monsterBattleId"].As<string>();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleId , monsterBattle, 1000);
            return monsterBattle;

        }

        public async Task<MonsterBattle> UpdateMonsterBattleAsync(MonsterBattleUpdateDto monsterBattleDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            {   
                monsterBattleId = monsterBattleDto.MonsterBattleId,
                endedAt = DateTime.Now,
                isFinalized= "true",
                lootItems=monsterBattleDto.LootItemsNames
            };
            if (monsterBattleDto.LootItemsNames.Length > 0){
                var lootQuery =@$"
                    MATCH ({_key}:{type}) WHERE ID({_key})=$monsterBattleId
                    MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key})
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
            string query = @$"
                MATCH ({_key}:{type}) WHERE ID({_key})=$monsterBattleId
                    SET {_key}.endedAt= $endedAt
                    SET {_key}.isFinalized= $isFinalized
                WITH {_key}";
            query +=ItemQueryBuilder.ConnectWithItemsFromList(monsterBattleDto.LootItemsNames, _key, "LOOT");
            query +=$@"  
                WITH {_key}
                MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key})-[:ATTACKING_PLAYER]->(player:Player)";
            query +=ReturnMonsterBattleWithAllItems(type, _key, "LOOT");
                
            var cursor=await session.RunAsync(query, parameters);
            var record=await cursor.SingleAsync();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleDto.MonsterBattleId , monsterBattle, 100);
            return monsterBattle;
        }
        
        public async Task<List<MonsterBattle>> GetMonsterBattlesAsync()
        {
            var session = _driver.AsyncSession();
            string query = $@"
                    MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key}:{type})-[:ATTACKING_PLAYER]->(player:Player)
                    ";
            query +=ReturnMonsterBattleWithAllItems(type, _key, "LOOT");
            var cursor=await session.RunAsync(query);
            var monsterBattles=new List<MonsterBattle>();
             await cursor.ForEachAsync(record =>
            {
                monsterBattles.Add(BuildMonsterBattle(record));
            });
            foreach (MonsterBattle monsterbattle in monsterBattles)
            {   if(!monsterbattle.IsFinalized)
                    await _cache.SetDataAsync(_key + monsterbattle.Id, monsterbattle, 100);
            }
            return monsterBattles;
        }
       public async Task<MonsterBattle> GetMonsterBattleAsync(int monsterBattleId)
        {
            var session = _driver.AsyncSession();
            string key = _key + monsterBattleId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var cachedMonsterBattle = await _cache.GetDataAsync<MonsterBattle>(key);
                return cachedMonsterBattle;          
            }
            var parameters = new { idn = monsterBattleId };
            string query = $@"
                MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key}:{type})-[:ATTACKING_PLAYER]->(player:Player)
                    WHERE ID({_key})=$idn";
            query +=ReturnMonsterBattleWithAllItems(type, _key,"LOOT");

            var cursor=await session.RunAsync(query, parameters);
            var record=await cursor.SingleAsync();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleId , monsterBattle, 100);
            return monsterBattle;
        } 
        
         public async Task<IResultCursor> DeleteMonsterBattle(int monsterBattleId)
        {
            var session = _driver.AsyncSession();
            string key = _key + monsterBattleId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                await _cache.DeleteAsync(key);         
            }
            var parameters = new { mbId = monsterBattleId};
            var query = @"
                MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-(n:MonsterBattle)-[:ATTACKING_PLAYER]->(n2:Player) 
                    WHERE id(n)=$mbId 
                DETACH DELETE n";
            return await session.RunAsync(query, parameters);
        }
        
        public static string ReturnMonsterBattleWithAllItems(string type, string identifier, string relationName)
        {
            
            string query= $@" 
                MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                    OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)";
            query += ItemQueryBuilder.ReturnObjectWithItems(type, identifier, relationName);
            query += $@", Id({identifier}) AS monsterBattleId, player, monster, monsterAttributes, COLLECT(DISTINCT{{
                    item: pLoot,
                    attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                }}) AS possibleLoot 
                ";
            return query;
        }
        
    }
}