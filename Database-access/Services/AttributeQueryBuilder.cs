using Microsoft.AspNetCore.Mvc.ModelBinding;
using Neo4j.Driver;

namespace Services
{
    public static class AttributeQueryBuilder
    {
        public static readonly string  _key = "attributes";
        public static readonly List<string> attributes =
        [
            "strength",
            "agility",
            "intelligence",
            "stamina",
            "faith",
            "experience",
            "level"
        ];
        public static string _type = "Attributes";
        public static string CreateAttributes(string identifier, string prefix="$")
        {
            string query = $"CREATE ({identifier}:Attributes {{";

            foreach (var attr in attributes)
            {
                query += $" {attr}: {prefix}{attr},";
            }

            query = query.TrimEnd(',') + " })";
            return query;
        }

        public static string AttachAttributes(string identifier, string connector = "HAS", string attributes = "attributes")
        {
            string query = $"Create ({identifier}-[:{connector}]->({attributes})) \n";
            return query;
        }

        public static string SetAttributes(string identifier="", string prefix="$")
        {
            string query = " ";
            foreach (var attr in attributes)
            {
                query += $"SET {identifier}.{attr} = {prefix}{attr} \n";
            }
            return query;
        }

        public static string AttributeAddition(string identifier="", string prefix="$")
        {
            string query = "";
            foreach (var attr in attributes)
            {
                query += $"SET {identifier}.{attr} = {identifier}.{attr} + {prefix}{attr} \n";
            }
            return query;
        }
    }     
}