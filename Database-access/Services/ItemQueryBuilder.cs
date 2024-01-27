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
        
    }     
}