using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Databaseaccess.Models;
using Neo4j.Driver;

[JsonPolymorphic]
[JsonDerivedType(typeof(Gear), "Gear")]
[JsonDerivedType(typeof(Consumable), "Consumable")]
public abstract class Item
{
    public string Name { get; set; } 
    public string Type { get; set; }
    public double Weight { get; set; } 
    public int Dimensions { get; set; }
    public int Value { get; set; }
    public Item(INode node)
    {
        Name = node["name"].As<string>();
        Type = node["type"].As<string>();
        Weight = node["weight"].As<double>();
        Dimensions = node["dimensions"].As<int>();
        Value = node["value"].As<int>();
    }
}
[Serializable]
public class Gear : Item
{
    public int Slot { get; set; }
    public int Level { get; set; }
    public string Quality { get; set; }
    public Attributes Attributes { get; set; }
    public Gear(INode itemNode, INode attributesNode) : base(itemNode)
    {
        Slot = itemNode["slot"].As<int>();
        if (itemNode.Properties.ContainsKey("level"))
            Level = itemNode["level"].As<int>();
        Quality = itemNode["quality"].As<string>();
        Attributes = new Attributes(attributesNode);
    }
}
[Serializable]
public class Consumable : Item
{
    public string Effect { get; set; }
    public Consumable(INode node) : base(node)
    {
        Effect = node["effect"].As<string>();
    }
}