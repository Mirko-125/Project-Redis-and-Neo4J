using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;
using Neo4j.Driver;

public class Marketplace
{
    public string Id { get; set; }
    public string Zone { get; set; } 
    public int ItemCount { get; set; } 
    public int RestockCycle { get; set; } 
    public List<Item> Items { get; set; }
    public Marketplace(INode marketNode, List<Dictionary<string, INode>> itemsNodeList = null)
    {
        Id = marketNode.ElementId;
        Zone = marketNode["zone"].As<string>();
        ItemCount = marketNode["itemCount"].As<int>();
        RestockCycle = marketNode["restockCycle"].As<int>();
        Items = [];

        if (itemsNodeList != null && itemsNodeList.Count > 0)
        {
            itemsNodeList.ForEach(itemAndAttributes => 
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
                Items.Add(item);
            });
        }
       
    }
}