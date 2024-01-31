using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class MonsterService
    {
        private readonly IDriver _driver;
        public readonly string type = "Monster";
        public readonly string _key = "monster";

        public static Monster BuildMonster(IRecord record)
        {
            var monsterNode = record["n"].As<INode>();
            var monsterAttributesNode = record["m"].As<INode>();
            var possibleLootNodeList = record["items"].As<List<Dictionary<string, INode>>>();
            return new(monsterNode, possibleLootNodeList, monsterAttributesNode);
        }

        public MonsterService(IDriver driver)
        {
            _driver = driver;
        }

        public static readonly List<string> props =
        [
            "name",
            "zone",
            "type",
            "imageURL",
            "status"
        ];

        public static object SetParams(MonsterCreateDto dto, string oldName="")
        {
             if (oldName == "")
            {
                oldName = dto.Name;
            }
            
            return new 
            {
                name = dto.Name,
                zone = dto.Zone,
                type = dto.Type,
                imageURL = dto.ImageURL,
                status = dto.Status,
                strength = dto.Attributes.Strength,
                agility = dto.Attributes.Agility,
                intelligence = dto.Attributes.Intelligence,
                stamina = dto.Attributes.Stamina,
                faith = dto.Attributes.Faith,
                experience = dto.Attributes.Experience,
                level = dto.Attributes.Level,
                possibleLootItems =  dto.PossibleLootNames
            };
        }

        public async Task<IResultCursor> CreateAsync(MonsterCreateDto monsterDto)
        {
            var session = _driver.AsyncSession();
            if(await MonsterExist(monsterDto.Name, session))
            {
                throw new Exception("Monster already exists.");
            }
            var parameters = SetParams(monsterDto);
            if (monsterDto.PossibleLootNames.Length > 0)
            {
                var possibleLootQuery =@"
                    MATCH (possibleLootItem: Item)
                        WHERE possibleLootItem.name
                            IN $possibleLootItems
                    RETURN possibleLootItem"
                ;
                var possibleLootItemResult = await session.RunAsync(possibleLootQuery, parameters);
                var possibleLootItems = await possibleLootItemResult.ToListAsync();

                if (possibleLootItems.Count != monsterDto.PossibleLootNames.Length)
                    throw new Exception("Some of the items don't exist");
            }
            string query = CreateQuery();
            query +=$"WITH {_key} \n";
            query+= AttributeQueryBuilder.CreateAttributes("attributes");
            query+=$@"
                WITH {_key}, attributes
                CREATE ({_key})-[:HAS]->(attributes)
                WITH {_key}
                ";
            query+=ItemQueryBuilder.ConnectWithItemsFromList(monsterDto.PossibleLootNames, _key, "POSSIBLE_LOOT");
            var result = await session.RunAsync(query,parameters);
            return result;
        }
       
        public async Task<IResultCursor> UpdateAsync(MonsterUpdateDto monster)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterExist(monster.OldName, session))
            {
                throw new Exception("Monster with this name doesn't exist.");
            }
            var parameters = new 
            { 
                oldName = monster.OldName,
                name = monster.Name,
                type = monster.Type,
                zone = monster.Zone,
                imageURL= monster.ImageURL,
                status = monster.Status,
                strength = monster.Attributes.Strength,
                agility = monster.Attributes.Agility,
                intelligence = monster.Attributes.Intelligence,
                stamina = monster.Attributes.Stamina,
                faith = monster.Attributes.Faith,
                experience = monster.Attributes.Experience,
                level = monster.Attributes.Level,
            };
            string query = @$"
                MATCH (n:{type})-[:HAS]->(m:Attributes) WHERE n.name = $oldName";
            query += SetQuery("n");
            query += AttributeQueryBuilder.SetAttributes("m");
            query += "RETURN n";
            var result= await session.RunAsync(query, parameters);
            return result;
        }
        public async Task<List<Monster>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            string query = $@"
                MATCH ({_key}:{type})
                OPTIONAL MATCH ({_key})-[:HAS]->(m:Attributes) 
                    ";
            query +=ReturnMonsterWithItems(type, _key,"POSSIBLE_LOOT");
            var cursor = await session.RunAsync(query);
            var monsters = new List<Monster>();

            await cursor.ForEachAsync(record =>
            {
                monsters.Add(BuildMonster(record));
            });
            return monsters;
        }
       public async Task<Monster> GetOneAsync(string monsterName)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterExist(monsterName, session))
            {
                throw new Exception("Monster with this name doesn't exist.");
            }
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE {_key}.name=$monsterName
                OPTIONAL MATCH ({_key})-[:HAS]->(m:Attributes) 
                ";
            query += ReturnMonsterWithItems(type, _key,"POSSIBLE_LOOT");

            var cursor = await session.RunAsync(query, new{monsterName});
            return BuildMonster(await cursor.SingleAsync());
        } 

         public async Task<IResultCursor> DeleteAsync(string monsterName)
        {
            var session = _driver.AsyncSession();
            if(!await MonsterExist(monsterName, session))
            {
                throw new Exception("Monster with this name doesn't exist.");
            }
            var query = $@"
                MATCH ({_key}:{type})-[r:HAS]->(m:Attributes) 
                    WHERE {_key}.name=$monsterName
                OPTIONAL MATCH ({_key})<-[a:ATTACKED_MONSTER]-(mb:MonsterBattle)
                DETACH DELETE m,mb,{_key}";
            return await session.RunAsync(query, new{monsterName});
        }

        public static string ReturnMonsterWithItems(string type, string identifier, string relationName)
        {
            string query = ItemQueryBuilder.ReturnObjectWithItems(type, identifier, relationName);
            query+= $@",m";
            return query;
        }
        public static string CreateQuery(string identifier="monster", string prefix="$")
        {
            string query = $"CREATE ({identifier}:Monster {{";

            foreach (var prop in props)
            {
                query += $" {prop}: {prefix}{prop},";
            }

            query = query.TrimEnd(',') + " })";
            return query;
        }
         public static string SetQuery(string identifier="", string prefix="$")
        {
            string query = " ";
            foreach (var prop in props)
            {
                query += $"SET {identifier}.{prop} = {prefix}{prop} \n";
            }
            return query;
        }
        public static async Task<bool> MonsterExist(string name, IAsyncSession sessions)
        {
            var session = sessions;
            string query= $@" 
                MATCH (monster:Monster) 
                    WHERE monster.name=$name
                RETURN COUNT(monster) AS count";
            var cursor = await session.RunAsync(query, new{name});
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