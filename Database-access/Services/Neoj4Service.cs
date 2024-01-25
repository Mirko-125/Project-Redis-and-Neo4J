using Neo4j.Driver;
namespace Services
{
    public static class QueryHelper {

        public static string BuildQuery(string connector, params string[] queries)
        {
            string query = ""; 
            foreach (string q in queries)
            {
                query += q;
                query += " " + connector + " ";
            }
            return query;
        }
    }
}