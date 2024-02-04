
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

        public async Task<IResultCursor> CreateAsync(MarketplaceCreateDto marketplaceDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                zone = marketplaceDto.Zone,
                itemCount = 0,
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
            string itemQuery = ItemQueryBuilder.FindItem(item, itemName);
            string addItemQuery = @$"
                WITH {item}
                    MATCH ({_key}:{type} {{zone: $zone}})
                    CREATE ({_key})-[:HAS]->({item}) 
                    SET {_key}.itemCount = COALESCE({_key}.itemCount, 0) + 1
                    ";
            string returnQuery = ItemQueryBuilder.ReturnSpecificObjectWithItems(type, _key);
            string query = itemQuery + addItemQuery + returnQuery;
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, parameters);
            var record = await cursor.SingleAsync();
            Marketplace market = BuildMarketplace(record);
            await _cache.SetDataAsync(_key + zoneName, market, 60);
            return market;
        }

        public async Task<List<Marketplace>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var keyExists = await _cache.CheckKeyAsync(_pluralKey);
            if (keyExists)
            {
                var cachedMarketplaces = await _cache.GetDataAsync<List<Marketplace>>(_pluralKey);
                return cachedMarketplaces;          
            }

            string query = $@"
                MATCH (n:{type})-[:HAS]->(i:Item) 
                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes)
                WITH n, ";
            query += ItemQueryBuilder.CollectItems();
            query += $" RETURN n, items";
            var cursor = await session.RunAsync(query);
            var marketplaces = new List<Marketplace>();
            Console.WriteLine(query);
            await cursor.ForEachAsync(record =>
            {
                marketplaces.Add(BuildMarketplace(record));
            });

            foreach (Marketplace market in marketplaces)
            {
                Console.WriteLine(market.Zone);
                await _cache.SetDataAsync(_key + market.Zone, market, 100);
            }
            await _cache.SetDataAsync(_pluralKey, marketplaces, 100);
            return marketplaces;
        }

        public async Task<Marketplace> GetOneAsync(string zone)
        {
            var session = _driver.AsyncSession();
            string key = _key + zone;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var cachedMarket = await _cache.GetDataAsync<Marketplace>(key);
                return cachedMarket;          
            }

            string query = $@"
                MATCH (n:{type})-[:HAS]->(i:Item) 
                    WHERE n.zone = $zone
                    OPTIONAL MATCH (i)-[r:HAS]->(a:Attributes) 
                    WITH n, ";
            query += ItemQueryBuilder.CollectItems();
            query += "Return n, items";
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, new {zone});
            Marketplace market = BuildMarketplace(await cursor.SingleAsync());
            await _cache.SetDataAsync(key, market, 100);
            return market;
        }

        public async Task<IResultCursor> UpdateAsync(MarketplaceUpdateDto dto)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            { 
                zone = dto.Zone,
                restockCycle = dto.RestockCycle
            };

            string query = @$"
                MATCH (n:{type}) WHERE n.zone = $zone
                   SET n.restockCycle = $restockCycle
                RETURN n";

            return await session.RunAsync(query, parameters);
        }

        public async Task<IResultCursor> DeleteAsync(string zone)
        {
            var session = _driver.AsyncSession();
            await _cache.DeleteAsync(_key + zone);
            var query = @$"MATCH (n:{type} {{zone: $zone}}) DETACH DELETE n";
            var parameters = new {zone};
            return await session.RunAsync(query, parameters);
        }

        public async Task<IResultCursor> DeleteOneItemAsync(string zone, string name)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (n:Marketplace)-[r:HAS]->(i:Item)
                    WHERE n.zone = $zone AND i.name = $name
                    OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                    DELETE r
                    SET n.itemCount = COALESCE(n.itemCount, 0) - 1
                    RETURN n
                ";
            var parameters = new {zone, name};
            return await session.RunAsync(query, parameters);
        }


        public async Task<bool> MarketplaceExist(string zone)
        {
            var session = _driver.AsyncSession();
            bool marketplaceExist = await MarketplaceExist(zone);
            if(!marketplaceExist)
            {
                throw new Exception($"Marketplace with this zone doesn't exist.");
            }
            var parameters = new { zone = zone };
            var checkQuery = $@"
                MATCH ({_key}:{type}) WHERE{_key}.zone=$zone
                    RETURN COUNT({_key}) AS count";

            var cursor = await session.RunAsync(checkQuery, parameters);
            var record = await cursor.SingleAsync();
            var countMarketplace = record["count"].As<int>();

            if (countMarketplace > 0)
            {
                return true;
            }
            return false;
        }
    }     
}