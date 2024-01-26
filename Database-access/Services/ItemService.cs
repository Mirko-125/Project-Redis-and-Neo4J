
using Cache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class ItemService
    {
        private readonly IDriver _driver;
        public readonly string _type = "Item";
        //public readonly string _pluralKey = "items";
        public readonly string _key = "item";

        private static object BuildItem(IRecord record)
        {
            var items = new List<object>();
            var item = record["n"].As<INode>();
            var connectedNodes = record["attributes"].As<INode>();
            items.Add(new { Item = item, ConnectedNodes = connectedNodes });
            return items;
        }

        public ItemService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<List<object>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            
            string query = $@"
                MATCH ({_key}:{_type})
                    OPTIONAL MATCH ({_key})-[:HAS]->(attributes:Attributes)     
                ";
            query += AttributesQueryBuilder.ReturnObjectWithAttributes(_type, _key);
            var cursor = await session.RunAsync(query);
            var items = new List<object>();
               
            await cursor.ForEachAsync(record =>
            {
                items.Add(BuildItem(record));
            });
           
            return items;            
   
        }

        public async Task<object> GetItemByNameAsync(string name)
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

        public async Task<object> GetItemsByTypeAsync(string type)
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH (n:{_type}) 
                    WHERE n.type = $type
                    OPTIONAL MATCH (n)-[r:HAS]->(attributes:Attributes) 
                RETURN  n, attributes";

            var cursor = await session.RunAsync(query, new {type = type});
            var items = new List<object>();
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