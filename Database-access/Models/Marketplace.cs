using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;
using Neo4j.Driver;

public class Marketplace
{
    //atributi
    public string Zone { get; set; } 
    public int ItemCount { get; set; } 
    public int RestockCycle {get; set;} 

    //veze
    public List<Item> Items { get; set; }
    public Marketplace(INode node)
    {
        Zone = node["Zone"].As<string>();
        ItemCount = node["ItemCount"].As<int>();
        RestockCycle = node["RestockCycle"].As<int>();
        Items = new List<Item>();
    }
}