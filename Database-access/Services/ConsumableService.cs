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

        public async Task<IResultCursor> AddConsumableAsync(ConsumableCreateDto consumableDto)
        {
            var session = _driver.AsyncSession();

           var parameters = new
            {
                name = consumableDto.Name,
                weight = consumableDto.Weight,
                type = consumableDto.Type,
                dimensions = consumableDto.Dimensions,
                value = consumableDto.Value,
                effect = consumableDto.Effect
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

        public async Task<IResultCursor> UpdateConsumableAsync(ConsumableUpdateDto consumableDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            { 
                consumableID = consumableDto.ConsumableID,
                name = consumableDto.Name, 
                type = consumableDto.Type,
                value = consumableDto.Value, 
                dimensions = consumableDto.Dimensions, 
                weight = consumableDto.Weight, 
                effect = consumableDto.Effect
            };

            string query = @$"
                MATCH (n:{_type}) WHERE ID(n)=$consumableID
                    SET n.name=$name
                    SET n.type=$type
                    SET n.value=$value 
                    SET n.dimensions=$dimensions 
                    SET n.weight=$weight 
                    SET n.effect=$effect 
                return n";

            return await session.RunAsync(query, parameters);
        }

    }     
}