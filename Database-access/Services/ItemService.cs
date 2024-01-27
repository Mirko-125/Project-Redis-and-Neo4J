using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class ItemService
    {
        private readonly IDriver _driver;
        public readonly string _type = "Item";
        public readonly string _key = "item";

        private static Item BuildItem(IRecord record)
        {
            var itemNode = record["n"].As<INode>();
            Item item;
            if(itemNode.Labels.Contains("Gear"))
            {
                INode attributesNode = record["attributes"].As<INode>();
                item = new Gear(itemNode, attributesNode); 
                   
            }
            else 
            {
                item = new Consumable(itemNode);
            }
            
            return item;
        }

        public ItemService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            
            string query = $@"
                MATCH ({_key}:{_type})
                    OPTIONAL MATCH ({_key})-[:HAS]->(attributes:Attributes)     
                ";
            query += AttributesQueryBuilder.ReturnObjectWithAttributes(_type, _key);
            var cursor = await session.RunAsync(query);
            var items = new List<Item>();
             
            await cursor.ForEachAsync(record =>
            {
                items.Add(BuildItem(record));
            });
            return items;            
        }

        public async Task<Item> GetItemByNameAsync(string name)
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH (n:{_type}) 
                    WHERE n.name = $name
                    OPTIONAL MATCH (n)-[r:HAS]->(attributes:Attributes) 
                RETURN  n, attributes";

            var cursor = await session.RunAsync(query, new {name = name});
            query += AttributesQueryBuilder.ReturnObjectWithAttributes(_type, _key);
            Console.WriteLine(query);
            var item = BuildItem(await cursor.SingleAsync());
            
            return item;
        }

        public async Task<List<Item>> GetItemsByTypeAsync(string type)
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH (n:{_type}) 
                    WHERE n.type = $type
                    OPTIONAL MATCH (n)-[r:HAS]->(attributes:Attributes) 
                RETURN  n, attributes";

            var cursor = await session.RunAsync(query, new {type = type});
            var items = new List<Item>();
            await cursor.ForEachAsync(record =>
            {
               items.Add(BuildItem(record));
            });
                
            return items;
        }


        public async Task<IResultCursor> DeleteItem(string name)
        {
            var session = _driver.AsyncSession();
            var query = @$"MATCH (n:{_type}) 
                                WHERE n.name = $name
                                OPTIONAL MATCH (n)-[:HAS]->(attributes:Attributes)
                           DETACH DELETE n, attributes";
            var parameters = new {name = name};
            return await session.RunAsync(query, parameters);
        }
    }     
}