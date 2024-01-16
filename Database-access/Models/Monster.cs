using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{

    public class Monster
    {
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Type{ get; set; }
        public string ImageURL { get; set; }
        public string Status { get; set; }
        public Attributes Attributes { get; set; }
        public List<Item> PossibleLoot { get; set; }
        public Monster(INode monster, List<Dictionary<string, INode>> possibleLootNodeList, INode attributes = null)
        {
            PossibleLoot = [];
            if (possibleLootNodeList.Count > 0)
            {
                possibleLootNodeList.ForEach(itemAndAttributes => 
                {
                    INode itemNode = itemAndAttributes["item"];
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
                    PossibleLoot.Add(item);
                });
            }
            Name = monster["name"].As<string>();
            Zone = monster["zone"].As<string>();
            Type = monster["type"].As<string>();
            ImageURL = monster["imageURL"].As<string>();
            Status = monster["status"].As<string>();
            if (attributes != null)
            {
                Attributes = new Attributes(attributes);
            }
            
        }
    }
}
