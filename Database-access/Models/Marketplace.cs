using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;
using Neo4j.Driver;
using Services;

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
        Items = ItemQueryBuilder.BuildItemList(itemsNodeList);
    }
}