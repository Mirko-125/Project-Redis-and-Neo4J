using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Databaseaccess.Models;
using Neo4j.Driver;
public abstract class Item
{
    public string Name { get; set; } 
    public string Type { get; set; }
    public double Weight { get; set; } // mozda bi trebalo u int da se stavi
    public int Dimensions { get; set; }
    public int Value { get; set; }

    // proveri ovo, izvini sto cakcam tvoje Jelena
    // mozda ovaj ne treba posto je abstraktna klasa
    public Item(INode node)
    {
        Name = node["name"].As<string>();
        Type = node["type"].As<string>();
        Weight = node["weight"].As<double>();
        Dimensions = node["dimensions"].As<int>();
        Value = node["value"].As<int>();
    }
}
public class Gear : Item
{
    public int Slot { get; set; }
    public int Level { get; set; }
    public string Quality { get; set; }
    public Attributes Attributes { get; set; } // not sure
    public Gear(INode node) : base(node)
    {
        Slot = node["slot"].As<int>();
        Level = node["level"].As<int>();
        Quality = node["quality"].As<string>();
        Attributes = new Attributes(node);
    }
}
public class Consumable : Item
{
    public string Effect { get; set; }
    public Consumable(INode node) : base(node)
    {
        Effect = node["effect"].As<string>();
    }
}