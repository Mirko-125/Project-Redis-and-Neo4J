using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class ConsumableService
    {
        private readonly IDriver _driver;
        public readonly string _type = "Consumable";
        public readonly string _key = "consumable";

        public ConsumableService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IResultCursor> CreateAsync(ConsumableCreateDto dto)
        {
            var session = _driver.AsyncSession();
            bool consumableExists = await ConsumableExist(dto.Name);
            if(consumableExists)
            {
                throw new Exception($"Consumable with name '{dto.Name}' already exists.");
            }
            var parameters = new
            {
                name = dto.Name,
                weight = dto.Weight,
                type = dto.Type,
                dimensions = dto.Dimensions,
                value = dto.Value,
                effect = dto.Effect
            };

            var query = $@"
                CREATE ({_key}:{_type}:Item {{
                    name: $name,
                    weight: $weight,
                    type: $type,
                    dimensions: $dimensions,
                    value: $value,
                    effect: $effect
                }})";

            var result = await session.RunAsync(query, parameters);
            return result;
        }

        public async Task<IResultCursor> UpdateAsync(ConsumableUpdateDto dto)
        {
            var session = _driver.AsyncSession();
            bool consumableExists = await ConsumableExist(dto.Name);
            if(!consumableExists)
            {
                throw new Exception($"Consumable with name '{dto.Name}' doesn't exists.");
            }
            var parameters = new 
            { 
                name = dto.Name, 
                type = dto.Type,
                value = dto.Value, 
                dimensions = dto.Dimensions, 
                weight = dto.Weight, 
                effect = dto.Effect
            };

            string query = @$"
                MATCH (n:{_type}) WHERE n.name=$name
                    SET n.type=$type
                    SET n.value=$value 
                    SET n.dimensions=$dimensions 
                    SET n.weight=$weight 
                    SET n.effect=$effect 
                return n";

            return await session.RunAsync(query, parameters);
        }

        public async Task<bool> ConsumableExist(string name)
        {
            var session = _driver.AsyncSession();
            var parameters = new { name = name };
            var checkQuery = $@"
                MATCH ({_key}:{_type}:Item {{name: $name}})
                    RETURN COUNT({_key}) AS count";

            var cursor = await session.RunAsync(checkQuery, parameters);
            var record = await cursor.SingleAsync();
            var countConsumable = record["count"].As<int>();

            if (countConsumable > 0)
            {
                return true;
            }
            return false;
        }

    }     
}