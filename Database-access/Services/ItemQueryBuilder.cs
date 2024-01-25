namespace Services
{
    public static class ItemQueryBuilder
    {
        public static string singularKey = "Item";
        public static string FindItem(string identifier)
        {
            string query = $@"
                MATCH ({identifier}:Item)
                        WHERE {identifier}.name = $item";
            return query;
        }
        public static string ReturnObjectWithItems(string type, string identifier, bool with = false)
        {
            string query = "";
            if (with)
                query += "WITH " + identifier + "\n";

            query += $@"
                MATCH ({identifier}:{type})-[:HAS]->(i:Item) 
                    OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                RETURN {identifier} as n, COLLECT({{
                    item: i,
                    attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                }}) AS items";

            return query;
        }
    }     
}