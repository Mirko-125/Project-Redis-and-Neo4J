using Neo4j.Driver;

namespace Services
{
    public static class ItemQueryBuilder
    {
        public static string singularKey = "item";
        public static string FindItem(string identifier, string itemName)
        {
            string query = $@"
                MATCH ({identifier}:Item)
                        WHERE {identifier}.name = '{itemName}'";
            return query;
        }

        public static string CollectItems(string itemIdentifier = "i", string attributeIdentifier="a", string itemsName="items")
        {
            string query = "";
            query += $@"
                COLLECT({{
                    item: {itemIdentifier},
                    attributes: CASE WHEN {itemIdentifier}:Gear THEN {attributeIdentifier} ELSE NULL END
                }}) AS {itemsName} ";
            
            return query;
        }
         public static string CollectDistinctItems(string itemIdentifier = "i", string attributeIdentifier="a", string itemsName="items")
        {
            string query = "";
            query += $@"
                COLLECT(DISTINCT{{
                    item: {itemIdentifier},
                    attributes: CASE WHEN {itemIdentifier}:Gear THEN {attributeIdentifier} ELSE NULL END
                }}) AS {itemsName} ";
            
            return query;
        }

        public static List<Item> BuildItemList(List<Dictionary<string, INode>> itemsNodeList)
        {
            List<Item> itemList = [];
            if (itemsNodeList != null && itemsNodeList.Count > 0)
            {
                itemsNodeList.ForEach(itemAndAttributes => 
                {
                    INode itemNode = itemAndAttributes["item"];
                    if (itemNode != null)
                    {
                        Item item;
                        if (itemNode.Labels.Contains("Gear")) 
                        {  
                            INode attributesNode = itemAndAttributes["attributes"];
                            item = new Gear(itemNode, attributesNode);
                        }
                        else 
                        {
                            item = new Consumable(itemNode);
                        }
                        itemList.Add(item);
                    }
                });
            }   

            return itemList;
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
                RETURN {identifier} as n, COLLECT({{
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