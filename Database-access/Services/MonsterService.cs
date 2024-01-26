using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class MonsterService
    {
        private readonly IDriver _driver;
        public readonly string type = "Monster";
        public readonly string _pluralKey = "monsters";
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

        public async Task<IResultCursor> AddAsync(MonsterCreateDto monsterDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                name = monsterDto.Name,
                zone = monsterDto.Zone,
                type = monsterDto.Type,
                imageURL= monsterDto.ImageURL,
                status=monsterDto.Status,
                possibleLootItems=monsterDto.PossibleLootNames
                
            };
            if (monsterDto.PossibleLootNames.Length > 0)
            {
                var possibleLootQuery =@"
                    MATCH (possibleLootItem: Item)
                        WHERE possibleLootItem.name
                            IN $possibleLootItems

                    return possibleLootItem"
                ;
                var possibleLootItemResult = await session.RunAsync(possibleLootQuery, parameters);
                var possibleLootItems = await possibleLootItemResult.ToListAsync();

                if (possibleLootItems.Count != monsterDto.PossibleLootNames.Length)
                    throw new Exception("Some of the items don't exist");
            }
            string query = $@"
                CREATE (n:{type} {{
                    name: $name,
                    zone: $zone,
                    type: $type,
                    imageURL: $imageURL,
                    status: $status
                }})
                WITH n";
            query+=AttributesService.CreateAttributes(monsterDto.Attributes);
            query+=$@"
                CREATE (n)-[:HAS]->(attributes)
                WITH n
                ";
            query+=ConnectWithItemsFromList(monsterDto.PossibleLootNames, "POSSIBLE_LOOT");
            var result = await session.RunAsync(query,parameters);
            return result;

        }
       
        public async Task<IResultCursor> UpdateMonsterAsync(MonsterUpdateDto monster)
        {
            var session = _driver.AsyncSession();

            var parameters = new { 
                        mId = monster.MonsterId,
                        zone = monster.Zone,
                        imageURL= monster.ImageURL,
                        status = monster.Status,
                    };

            string query = @$"
                MATCH (n:{type})-[:HAS]->(m:Attributes) WHERE ID(n)=$mId
                SET n.zone= $zone
                SET n.imageURL= $imageURL
                SET n.status= $status";
            query += AttributesService.UpdateAttributes(monster.Attributes);
            query += "RETURN n";
            var result= await session.RunAsync(query, parameters);
            return result;
        }
        public async Task<List<Monster>> GetMonstersAsync()
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
       public async Task<Monster> GetMonsterAsync(int monsterId)
        {
            var session = _driver.AsyncSession();
            var parameters = new { idn = monsterId };
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE id({_key})=$idn
                OPTIONAL MATCH ({_key})-[:HAS]->(m:Attributes) 
                ";
            query +=ReturnMonsterWithItems(type, _key,"POSSIBLE_LOOT");

            var cursor = await session.RunAsync(query, parameters);
            return BuildMonster(await cursor.SingleAsync());
        } 

         public async Task<IResultCursor> DeleteMonster(int monsterId)
        {
            var session = _driver.AsyncSession();
            var parameters = new { nId = monsterId };
            var query = @"
                MATCH (n:Monster)-[r:HAS]->(m:Attributes) 
                    WHERE Id(n)=$nId
                OPTIONAL MATCH (n)<-[a:ATTACKED_MONSTER]-(mb:MonsterBattle)
                DETACH DELETE m,mb,n";
            return await session.RunAsync(query, parameters);
        }

        public static string ConnectWithItemsFromList( string[] listOfItemsNames, string relationName)
        {
           
            string itemsNames = string.Join(", ", listOfItemsNames.Select(name => $"\"{name}\""));
            string query = $@"
                MATCH (possibleLootItem: Item)
                        WHERE possibleLootItem.name
                            IN [{itemsNames}]
                WITH n,collect(possibleLootItem) as possibleLootItemsList            
                   
                FOREACH (item IN possibleLootItemsList |
                    CREATE (n)-[:{relationName}]->(item)
                )";

            return query;
        }
        public static string ReturnMonsterWithItems(string type, string identifier, string relationName)
        {
            string query = ItemQueryBuilder.ReturnObjectWithItems(type, identifier, relationName);
            query+= $@",m";
            return query;
        }
    }
}