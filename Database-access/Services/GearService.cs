
using Cache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;

namespace Services
{
    public class GearService
    {
        private readonly IDriver _driver;
        public readonly string _type = "Gear";
        //public readonly string _pluralKey = "gears";
        public readonly string _key = "gear";

        public GearService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IResultCursor> AddGearAsync(GearCreateDto gearDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new
            {
                name = gearDto.Name,
                weight = gearDto.Weight,
                type = gearDto.Type,
                dimensions = gearDto.Dimensions,
                value = gearDto.Value,
                slot = gearDto.Slot,   
                level = gearDto.Level,
                quality = gearDto.Quality,

                //attributes
                strength = gearDto.Attributes.Strength,
                agility = gearDto.Attributes.Agility,
                intelligence = gearDto.Attributes.Intelligence,
                stamina = gearDto.Attributes.Stamina,
                faith = gearDto.Attributes.Faith,
                experience = gearDto.Attributes.Experience,
                levelAttributes = gearDto.Attributes.Level
            };

            var query = $@"
                CREATE ({_key}:Item:{_type} {{
                    name: $name,
                    weight: $weight,
                    type: $type,
                    dimensions: $dimensions,
                    value: $value,
                    slot: $slot,
                    level: $level,
                    quality: $quality
                }})
                CREATE (attributes:Attributes {{ 
                    strength: $strength, 
                    agility: $agility, 
                    intelligence: $intelligence, 
                    stamina: $stamina, 
                    faith: $faith, 
                    experience: $experience, 
                    levelAttributes: $levelAttributes
                }})
                CREATE (gear)-[:HAS]->(attributes)
                ";

            var result = await session.RunAsync(query, parameters);
            return result;

        }

        public async Task<IResultCursor> UpdateGearAsync(GearUpdateDto gearDto)
        {
            var session = _driver.AsyncSession();

            var parameters = new 
            { 
                gearID = gearDto.GearID,
                name = gearDto.Name,
                type = gearDto.Type, 
                value = gearDto.Value, 
                dimensions = gearDto.Dimensions, 
                weight = gearDto.Weight, 
                slot = gearDto.Slot,
                level = gearDto.Level,
                quality = gearDto.Quality,

                strength = gearDto.Attributes.Strength,
                agility = gearDto.Attributes.Agility ,
                intelligence = gearDto.Attributes.Intelligence,
                stamina = gearDto.Attributes.Stamina,
                faith = gearDto.Attributes.Faith,
                experience = gearDto.Attributes.Experience ,
                levelAttributes = gearDto.Attributes.Level                
            };

            string query = @$"
                MATCH (n:{_type})-[:HAS]->(attributes:Attributes) WHERE ID(n)=$gearID
                    SET n.name=$name
                    SET n.type=$type
                    SET n.value=$value 
                    SET n.dimensions=$dimensions 
                    SET n.weight=$weight 
                    SET n.slot=$slot
                    SET n.level=$level
                    SET n.quality=$quality

                    SET attributes.strength= $strength
                    SET attributes.agility= $agility
                    SET attributes.intelligence= $intelligence
                    SET attributes.stamina= $stamina
                    SET attributes.faith= $faith
                    SET attributes.experience= $experience
                    SET attributes.levelAttributes= $levelAttributes

                return n";

            return await session.RunAsync(query, parameters);
        }

    }     
}