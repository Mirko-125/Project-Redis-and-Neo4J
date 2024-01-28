using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class GearService
    {
        private readonly IDriver _driver;
        public readonly string _type = "Gear";
        public readonly string _key = "gear";

        public GearService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IResultCursor> CreateAsync(GearCreateDto dto)
        {
            var session = _driver.AsyncSession();
            bool gearExists = await GearExist(dto.Name);
            if(gearExists)
            {
                throw new Exception($"Gear with name '{dto.Name}' already exists.");
            }
            var parameters = new
            {
                name = dto.Name,
                weight = dto.Weight,
                type = dto.Type,
                dimensions = dto.Dimensions,
                value = dto.Value,
                slot = dto.Slot,   
                quality = dto.Quality,

                //attributes
                strength = dto.Attributes.Strength,
                agility = dto.Attributes.Agility,
                intelligence = dto.Attributes.Intelligence,
                stamina = dto.Attributes.Stamina,
                faith = dto.Attributes.Faith,
                experience = dto.Attributes.Experience,
                levelAttributes = dto.Attributes.Level
            };

            var query = $@"
                CREATE ({_key}:Item:{_type} {{
                    name: $name,
                    weight: $weight,
                    type: $type,
                    dimensions: $dimensions,
                    value: $value,
                    slot: $slot,
                    quality: $quality
                }})
                CREATE (attributes:Attributes {{ 
                    strength: $strength, 
                    agility: $agility, 
                    intelligence: $intelligence, 
                    stamina: $stamina, 
                    faith: $faith, 
                    experience: $experience, 
                    level: $levelAttributes
                }})
                CREATE (gear)-[:HAS]->(attributes)
                ";

            var result = await session.RunAsync(query, parameters);
            return result;
        }

        public async Task<IResultCursor> UpdateGearAsync(GearUpdateDto dto)
        {
            var session = _driver.AsyncSession();
            bool gearExists = await GearExist(dto.Name);
            if(!gearExists)
            {
                throw new Exception($"Gear with name '{dto.Name}' doesn't exists.");
            }
            var parameters = new 
            { 
                name = dto.Name,
                type = dto.Type, 
                value = dto.Value, 
                dimensions = dto.Dimensions, 
                weight = dto.Weight, 
                slot = dto.Slot,
                quality = dto.Quality,

                strength = dto.Attributes.Strength,
                agility = dto.Attributes.Agility ,
                intelligence = dto.Attributes.Intelligence,
                stamina = dto.Attributes.Stamina,
                faith = dto.Attributes.Faith,
                experience = dto.Attributes.Experience ,
                levelAttributes = dto.Attributes.Level                
            };

            string query = @$"
                MATCH (n:{_type})-[:HAS]->(attributes:Attributes) WHERE n.name=$name
                    SET n.type=$type
                    SET n.value=$value 
                    SET n.dimensions=$dimensions 
                    SET n.weight=$weight 
                    SET n.slot=$slot
                    SET n.quality=$quality

                    SET attributes.strength= $strength
                    SET attributes.agility= $agility
                    SET attributes.intelligence= $intelligence
                    SET attributes.stamina= $stamina
                    SET attributes.faith= $faith
                    SET attributes.experience= $experience
                    SET attributes.level= $levelAttributes

                return n";

            return await session.RunAsync(query, parameters);
        }

        public async Task<bool> GearExist(string name)
        {
            var session = _driver.AsyncSession();
            var parameters = new { name = name };
            var checkQuery = $@"
                MATCH ({_key}:{_type}:Item {{name: $name}})
                    RETURN COUNT({_key}) AS count";

            var cursor = await session.RunAsync(checkQuery, parameters);
            var record = await cursor.SingleAsync();
            var countGear = record["count"].As<int>();

            if (countGear > 0)
            {
                return true;
            }
            return false;
        }
    }     
}