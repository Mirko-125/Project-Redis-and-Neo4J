using Databaseaccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using ServiceStack;

namespace Services
{
    public class ClassService
    {
        private readonly IDriver _driver;

        public ClassService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task CreateAsync(ClassDto dto)
        {
            var session = _driver.AsyncSession();
            string query = @"
                CREATE (class:Class { name: $nameValue})"
                + AttributeQueryBuilder.CreateAttributes("base", "$base")
                + AttributeQueryBuilder.CreateAttributes("levelup", "$levelup")
                + @$"
                CREATE (class)-[:HAS_BASE_ATTRIBUTES]->(base)
                CREATE (class)-[:LEVEL_GAINS_ATTRIBUTES]->(levelup)";

            var parameters = SetParameters(dto);
            await session.RunAsync(query, parameters);  
        }

        public async Task AddPermissionAsync(string className, string abilityName, int level)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (n:Class) WHERE n.name = $className
                MATCH (m:Ability) WHERE m.name = $abilityName
                MERGE (n)-[permission:PERMITS]->(m)
                SET permission.level = $level";

            var parameters = new
            {
                className,
                abilityName,
                level
            };

            await session.RunAsync(query, parameters);
        }

        public async Task UpdateAsync(UpdateClassDto dto)
        {
            var session = _driver.AsyncSession();
            var parameters = SetParameters(dto, dto.OldName);
            string query = @"
                MATCH (base)<-[:HAS_BASE_ATTRIBUTES]-(class:Class)-[:LEVEL_GAINS_ATTRIBUTES]->(levelup)
                    WHERE class.name = $oldName
                    SET class.name = $nameValue
                    ";
            query += AttributeQueryBuilder.SetAttributes("base", "$base");
            query += AttributeQueryBuilder.SetAttributes("levelup", "$levelup");
            await session.RunAsync(query, parameters);
        }

        public async Task<List<Class>> GetAll()
        {
            var session = _driver.AsyncSession();
            string query = @"
                MATCH (class:Class) RETURN class as n ";
            var cursor = await session.RunAsync(query);
            List<Class> classList = [];
            await cursor.ForEachAsync(record =>
            {
                classList.Add(new Class(record["n"].As<INode>()));
            });
            return classList;
        }

        public async Task DeleteAsync(string name)
        {
            var session = _driver.AsyncSession();
            var query = @"
                MATCH (c:Class) where c.name = $name
                    OPTIONAL MATCH (c)-[r]-()
                DELETE r,c";
            await session.RunAsync(query, new {name});
        }

        public static object SetParameters(ClassDto dto, string oldName = "")
        {
            if (oldName == "")
            {
                oldName = dto.Name;
            }

            return new
            {
                oldName,
                nameValue = dto.Name,
                basestrength = dto.BaseAttributes.Strength,
                baseagility = dto.BaseAttributes.Agility,
                baseintelligence = dto.BaseAttributes.Intelligence,
                basestamina = dto.BaseAttributes.Stamina,
                basefaith = dto.BaseAttributes.Faith,
                baseexperience = 0,
                baselevel = 0,
                levelupstrength = dto.LevelGainAttributes.Strength,
                levelupagility = dto.LevelGainAttributes.Agility,
                levelupintelligence = dto.LevelGainAttributes.Intelligence,
                levelupstamina = dto.LevelGainAttributes.Stamina,
                levelupfaith = dto.LevelGainAttributes.Faith,
                levelupexperience = 0,
                leveluplevel = 0
            }; 
        }
        
    }
}