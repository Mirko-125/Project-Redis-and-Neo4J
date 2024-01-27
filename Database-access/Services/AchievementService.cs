using Neo4j.Driver;
using Databaseaccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;

namespace Services
{
    public class AchievementService
    {
        private readonly IDriver _driver;

        public AchievementService(IDriver driver)
        {
            _driver = driver;
        }

        public static readonly List<string> props =
        [
            "name",
            "type",
            "points",
            "conditions"
        ];

        public async Task<Achievement> CreateAsync(AchievementDTO dto)
        {
            var session = _driver.AsyncSession();
            string query = CreateQuery();
            query += " RETURN n";
            var parameters = SetParams(dto);
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, parameters);
            var record = await cursor.SingleAsync();
            Achievement achievement = new(record["n"].As<INode>());
            return achievement;
        }

        public async Task<List<Achievement>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            string query = @"
                MATCH (n:Achievement) RETURN n";
            var cursor = await session.RunAsync(query);
            List<Achievement> achievements = [];
            await cursor.ForEachAsync(record =>
            {
                achievements.Add(new Achievement(record["n"].As<INode>()));
            });

            return achievements;
        }

        public async Task GiveAchievementAsync(string playerName, string achievementName)
        {
            var session = _driver.AsyncSession();
            var query = @"
                MATCH (p:Player)
                    WHERE p.name = $playerName
                MATCH (a:Achievement)
                    WHERE a.name = $achievementName
                    
                MERGE (p)-[:ACHIEVED]->(a)";
            await session.RunAsync(query, new {playerName, achievementName});
            
        }

        public async Task<Achievement> UpdateAsync(UpdateAchievementDto dto)
        {
            var session = _driver.AsyncSession();
            string query = $"Match (n:Achievement) WHERE n.name = $oldName \n";
            query += UpdateQuery();
            query += "RETURN n";
            var parameters = SetParams(dto, dto.OldName);
            var cursor = await session.RunAsync(query, parameters);
            var record = await cursor.SingleAsync();
            var node = record["n"].As<INode>();
            return new Achievement(node);
        }

        public async Task DeleteAsync(string name)
        {
            var session = _driver.AsyncSession();
            var query = @"
                MATCH (a:Achievement) where a.name=$name
                    OPTIONAL MATCH (a)-[r]-()

                DELETE r,a";
            await session.RunAsync(query, new {name});
        }

        public static object SetParams(AchievementDTO dto, string oldName="")
        {
            if (oldName == "")
            {
                oldName = dto.Name;
            }

            return new 
            {
                oldName,
                name = dto.Name,
                type = dto.Type,
                points = dto.Points,
                conditions = dto.Conditions,
            };
        }

        public static string CreateQuery(string identifier="n", string prefix="$")
        {
            string query = $"CREATE ({identifier}:Achievement {{";

            foreach (var prop in props)
            {
                query += $" {prop}: {prefix}{prop},";
            }

            query = query.TrimEnd(',') + " })";
            return query;
        }

        public static string UpdateQuery(string identifier="n", string prefix="$")
        {
            string query = "";

            foreach (var prop in props)
            {
                query += $"SET {identifier}.{prop} = {prefix}{prop} \n";
            }
            
            return query;
        }

    }
}