using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;

public class Marketplace
{
    //atributi
    public string Zone { get; set; } 
    public int ItemCount { get; set; } 
    public int RestockCycle {get; set;} 

    //veze
    public List<Item> Items { get; set; }



}