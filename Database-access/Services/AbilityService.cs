using Databaseaccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using ServiceStack;

namespace Services
{
    public class AbilityService
    {
        private readonly IDriver _driver;
        public static readonly string  _key = "ability";
        public static readonly List<string> props =
        [
            "name",
            "damage",
            "cooldown",
            "range",
            "special",
            "heal"
        ];
        public static string _type = "Ability";

        public AbilityService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task CreateAsync(AbilityDTO dto)
        {
            var session = _driver.AsyncSession();
            string query = CreateQuery();
            var parameters = SetParams(dto);
            await session.RunAsync(query, parameters);
        }
        public async Task<Ability> UpdateAsync(UpdateAbilityDto dto)
        {
            var session = _driver.AsyncSession();
            string query = $"Match (n:Ability) WHERE n.name = '{dto.OldName}' \n";
            query += UpdateQuery();
            query += "RETURN n";
            var parameters = SetParams(dto);
            var cursor = await session.RunAsync(query, parameters);
            var record = await cursor.SingleAsync();
            var node = record["n"].As<INode>();
            return new Ability(node);
        }
        public async Task AssignAbilityAsync(string abilityName, string playerName)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (n:Ability)
                    WHERE n.name = '{abilityName}'
                MATCH (m:Player) WHERE m.name = '{playerName}'

                CREATE (m)-[:KNOWS]->(n)";
            await session.RunAsync(query);
            await session.DisposeAsync();
        }
        public async Task DeleteAsync(string abilityName)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (c:Ability) where c.name = '{abilityName}'
                    OPTIONAL MATCH (c)-[r]-()

                DELETE r,c";
            await session.RunAsync(query);
        }

        public async Task<List<Ability>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var query = "MATCH (n:Ability) RETURN n";
            var cursor = await session.RunAsync(query);
            List<Ability> abilities = [];
            await cursor.ForEachAsync(record =>
            {
                abilities.Add(new Ability(record["n"].As<INode>()));
            });
            return abilities;
        }
        public static string UpdateQuery(string identifier="n", string prefix="$")
        {
            string query = "";

            foreach (var prop in props)
            {
                query += $"SET {identifier}.{prop} = {prefix}{prop}" + " \n";
            }
            
            return query;
        }
        public static object SetParams(AbilityDTO dto)
        {
            return new 
            {
                name = dto.Name,
                damage = dto.Damage,
                cooldown = dto.Cooldown,
                range = dto.Range,
                special = dto.Special,
                heal = dto.Heal
            };
        }
        public static string CreateQuery(string identifier="n", string prefix="$")
        {
            string query = $"CREATE ({identifier}:Ability {{";

            foreach (var prop in props)
            {
                query += $" {prop}: {prefix}{prop},";
            }

            query = query.TrimEnd(',') + " })";
            return query;
        }

    }     
}