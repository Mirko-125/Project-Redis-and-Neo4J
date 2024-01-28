using Cache;
using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class MonsterBattleService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "MonsterBattle";
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
            _cache = cache;
        }

        public async Task<MonsterBattle> CreateAsync(MonsterBattleCreateDto mb)
        {
            var session = _driver.AsyncSession();
            if(!await PlayerService.PlayerExist(mb.PlayerName, session))
            {
                throw new Exception("Player with this name doesn't exist.");
            }
            if(!await MonsterService.MonsterExist(mb.MonsterName, session))
            {
                throw new Exception("Monster with this name doesn't exist.");
            }
             var parameters = new
            {
                startedAt=DateTime.Now,
                endedAt="--",
                isFinalized="false",
                playername=mb.PlayerName,
                monstername=mb.MonsterName
            };
            string query = $@"
                CREATE ({_key}:{type} {{
                    startedAt: $startedAt,
                    endedAt: $endedAt,
                    isFinalized: $isFinalized
                }}) 
                WITH {_key}
                MATCH (monster:Monster) WHERE monster.name=$monstername
                MATCH (player:Player) WHERE player.name=$playername
                CREATE (monster)<-[:ATTACKED_MONSTER]-({_key})-[:ATTACKING_PLAYER]->(player)
                WITH {_key}, monster, player"
            ;
            query += ReturnMonsterBattleWithAllItems(type, _key, "LOOT");
            var cursor=await session.RunAsync(query, parameters);
            var record=await cursor.SingleAsync();
            var monsterBattleId=record["monsterBattleId"].As<string>();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleId, monsterBattle, 1000);
            return monsterBattle;

        }

        public async Task<MonsterBattle> Finalize(MonsterBattleUpdateDto monsterBattleDto)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterBattleExist(monsterBattleDto.MonsterBattleId, session))
            {
                throw new Exception("Monster battle with this ID doesn't exist.");
            }
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
                    RETURN lootItem"
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
            if(monsterBattleDto.LootItemsNames.Length > 0)
            {
                query +=ItemQueryBuilder.ConnectWithItemsFromList(monsterBattleDto.LootItemsNames, _key, "LOOT");
            }
            query +=$@"  
                WITH {_key}
                MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key})-[:ATTACKING_PLAYER]->(player:Player)";
            query +=ReturnMonsterBattleWithAllItems(type, _key, "LOOT");         
            var cursor=await session.RunAsync(query, parameters);
            var record=await cursor.SingleAsync();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleDto.MonsterBattleId, monsterBattle, 100);
            return monsterBattle;
        }
        
        public async Task<List<MonsterBattle>> GetAllAsync()
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
       public async Task<MonsterBattle> GetOneAsync(int monsterBattleId)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterBattleExist(monsterBattleId, session))
            {
                throw new Exception("Monster battle with this ID doesn't exist.");
            }
            string key = _key + monsterBattleId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var cachedMonsterBattle = await _cache.GetDataAsync<MonsterBattle>(key);
                return cachedMonsterBattle;          
            }
            string query = $@"
                MATCH (monster:Monster)<-[:ATTACKED_MONSTER]-({_key}:{type})-[:ATTACKING_PLAYER]->(player:Player)
                    WHERE ID({_key})=$monsterBattleId";
            query +=ReturnMonsterBattleWithAllItems(type, _key,"LOOT");
            var cursor=await session.RunAsync(query, new{monsterBattleId});
            var record=await cursor.SingleAsync();
            MonsterBattle monsterBattle= BuildMonsterBattle(record);
            await _cache.SetDataAsync(_key + monsterBattleId , monsterBattle, 100);
            return monsterBattle;
        } 
        
         public async Task<IResultCursor> DeleteAsync(int monsterBattleId)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterBattleExist(monsterBattleId, session))
            {
                throw new Exception("Monster battle with this ID doesn't exist.");
            }
            string key = _key + monsterBattleId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                await _cache.DeleteAsync(key);         
            }
            var query = $@"
                MATCH (n1:Monster)<-[:ATTACKED_MONSTER]-({_key}:{type})-[:ATTACKING_PLAYER]->(n2:Player) 
                    WHERE Id({_key})=$monsterBattleId 
                DETACH DELETE {_key}";
            return await session.RunAsync(query, new{monsterBattleId});
        }
        
        public static string ReturnMonsterBattleWithAllItems(string type, string identifier, string relationName)
        {
            string query= $@" 
                MATCH (monster)-[:HAS]->(monsterAttributes:Attributes)
                OPTIONAL MATCH (monster)-[:POSSIBLE_LOOT]->(pLoot:Item)
                    OPTIONAL MATCH (pLoot)-[:HAS]->(att:Attributes)";
            query += ItemQueryBuilder.ReturnObjectWithItems(type, identifier, relationName);
            query += $@", Id({identifier}) AS monsterBattleId, player, monster, monsterAttributes, COLLECT(DISTINCT{{
                    item: pLoot,
                    attributes: CASE WHEN pLoot:Gear THEN att ELSE NULL END
                }}) AS possibleLoot 
                ";
            return query;
        }
        
        public static async Task<bool> MonsterBattleExist(int monsterBattleId, IAsyncSession sessions)
        {
            var session = sessions;
            string query= $@" 
                MATCH (n:MonsterBattle) 
                    WHERE Id(n)=$monsterBattleId 
                RETURN COUNT(n) AS count";
            var cursor = await session.RunAsync(query, new{monsterBattleId});
            var record = await cursor.SingleAsync();
            var br = record["count"].As<int>();
            if(br > 0)
            { 
                return true;
            }

            return false;
        }
    }
}