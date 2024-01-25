
using Cache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class MarketplaceService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "Marketplace";
        public readonly string _pluralKey = "marketplaces";
        public readonly string _key = "marketplace";

        private static Marketplace BuildMarketplace(IRecord record)
        {
            var marketNode = record["n"].As<INode>();
            var itemsNodeList = record["items"].As<List<Dictionary<string, INode>>>();
            return new Marketplace(marketNode, itemsNodeList);
        }

        public MarketplaceService(IDriver driver, RedisCache cache)
        {
            _driver = driver;
            _cache = cache;
        }

        public async Task<IResultCursor> AddAsync(MarketplaceCreateDto marketplaceDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                zone = marketplaceDto.Zone,
                itemCount = marketplaceDto.ItemCount,
                restockCycle = marketplaceDto.RestockCycle
            };

            var query = $@"
                CREATE (n:{type} {{
                    zone: $zone,
                    itemCount: $itemCount,
                    restockCycle: $restockCycle
                }})";

            var result = await session.RunAsync(query, parameters);
            return result;

        }

        public async Task<Marketplace> AddItem(string zoneName, string itemName)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            {  
                item = itemName,
                zone = zoneName
            };

            string item = ItemQueryBuilder.singularKey;
            string itemQuery = ItemQueryBuilder.FindItem(item);
            string addItemQuery = @$"
                WITH {item}
                    MATCH ({_key}:{type} {{zone: $zone}})
                    MERGE ({_key})-[:HAS]->({item}) ";
            string returnQuery = ItemQueryBuilder.ReturnObjectWithItems(type, _key, true);
            string query = itemQuery + addItemQuery + returnQuery;
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, parameters);
            var record = await cursor.SingleAsync();
            Marketplace market = BuildMarketplace(record);
            return market;
        }

        public async Task<List<Marketplace>> GetMarketplacesAsync()
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH ({_key}:{type})-[:HAS]->(i:Item) 
                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                    ";
            query += ItemQueryBuilder.ReturnObjectWithItems(type, _key);
            var cursor = await session.RunAsync(query);
            var marketplaces = new List<Marketplace>();
            Console.WriteLine(query);
            await cursor.ForEachAsync(record =>
            {
                marketplaces.Add(BuildMarketplace(record));
            });

            return marketplaces;
        }

        public async Task<Marketplace> GetMarketplaceAsync(string zone)
        {
            var session = _driver.AsyncSession();

            string query = $@"
                MATCH (n:{type})-[:HAS]->(i:Item) 
                    WHERE n.zone = $zone
                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes) ";
            query += ItemQueryBuilder.ReturnObjectWithItems(type, _key);
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, new {zone = zone});
            return BuildMarketplace(await cursor.SingleAsync());
        }

        public async Task<IResultCursor> UpdateMarketplaceAsync(MarketplaceUpdateDto dto)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            { 
                marketplaceID = dto.MarketplaceID,
                zone = dto.Zone,
                restockCycle = dto.RestockCycle
            };

            string query = @$"
                MATCH (n:{type}) WHERE ID(n)=$marketplaceID
                    SET n.zone = $zone
                    SET n.restockCycle = $restockCycle
                RETURN n";

            return await session.RunAsync(query, parameters);
        }

        public async Task<IResultCursor> DeleteMarketplace(string zone)
        {
            var session = _driver.AsyncSession();
            var query = @$"MATCH (n:{type} {{zone: $zone}}) DETACH DELETE n";
            var parameters = new {zone = zone};
            return await session.RunAsync(query, parameters);
        }
    }     
}