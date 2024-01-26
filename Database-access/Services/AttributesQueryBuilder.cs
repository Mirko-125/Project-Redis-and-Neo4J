namespace Services
{
    public static class AttributesQueryBuilder
    {
        public static string singularKey = "Attributes";
        public static string FindAttributes(string identifier, int attributesID)
        {
            string query = $@"
                MATCH ({identifier}:Attributes)
                        WHERE {identifier}.id = ${attributesID}
                ";
            return query;
        }
        public static string ReturnSpecificObjectWithAttributes(string type, string identifier)
        {
            string query = "";
            query += "WITH " + identifier + "\n";
            query += ReturnObjectWithAttributes(type, identifier);
            return query;
        }
        public static string ReturnObjectWithAttributes(string type, string identifier)
        {
            string query = "";
            query += $@"
                MATCH ({identifier}:{type}) 
                    OPTIONAL MATCH ({identifier})-[:HAS]->(attributes:Attributes)
                RETURN {identifier} as n, attributes";
            
            return query;
        }
    }     
}