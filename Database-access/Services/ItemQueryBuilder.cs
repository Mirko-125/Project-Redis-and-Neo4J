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
        public static string ReturnSpecificObjectWithItems(string type, string identifier)
        {
            string query = "";
            query += "WITH " + identifier + "\n";
            query += ReturnObjectWithItems(type, identifier,"HAS");
            return query;
        }
        public static string ReturnObjectWithItems(string type, string identifier, string relationName)
        {
            string query = "";
            query += $@"
                OPTIONAL MATCH ({identifier}:{type})-[:{relationName}]->(i:Item) 
                    OPTIONAL MATCH (i)-[:HAS]->(a:Attributes)
                RETURN {identifier} as n, COLLECT(DISTINCT{{
                    item: i,
                    attributes: CASE WHEN i:Gear THEN a ELSE NULL END
                }}) AS items";
            
            return query;
        }
         public static string ConnectWithItemsFromList( string[] listOfItemsNames,string identifier, string relationName)
        {
           
            string itemsNames = string.Join(", ", listOfItemsNames.Select(name => $"\"{name}\""));
            string query = $@"
                MATCH (itemm: Item)
                        WHERE itemm.name
                            IN [{itemsNames}]
                WITH {identifier}, COLLECT(itemm) as itemmList            
                   
                FOREACH (item IN itemmList |
                    CREATE ({identifier})-[:{relationName}]->(item)
                )
                ";

            return query;
        }
    }     
}