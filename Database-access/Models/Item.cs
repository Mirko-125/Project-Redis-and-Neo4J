using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;

public class Item
{
    public string Name { get; set; } 
    public double Weight { get; set; } // mozda bi trebalo u int da se stavi
    public int Dimensions { get; set; }
    public int Value { get; set; }
    public Inventory Inventory { get; set; }
}
public class Gear : Item
{
    public int Slot { get; set; }
    public string Type { get; set; }
    public int Level { get; set; }
    public string Quality { get; set; }
    public Equipment Equipment { get; set; }
    public Class Class { get; set; }
    public Attribute Attribute { get; set; } // not sure
}
public class Consumable : Item
{
    public string Type { get; set; }
    public string Effect { get; set; }
}