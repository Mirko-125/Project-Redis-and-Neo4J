using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Databaseaccess.Models;

public class Item
{
    public string Name { get; set; } 
    public string Type { get; set; }
    public double Weight { get; set; } // mozda bi trebalo u int da se stavi
    public int Dimensions { get; set; }
    public int Value { get; set; }
}
public class Gear : Item
{
    public int Slot { get; set; }
    public int Level { get; set; }
    public string Quality { get; set; }
    public Attributes Attributes { get; set; } // not sure
}
public class Consumable : Item
{
    public string Effect { get; set; }
}