using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;

public class Trade
{
    public bool IsFinalized { get; set; } 
    public int ReceiverGold { get; set; } 
    public int RequesterGold { get; set; } 
    public string StartedAt { get; set; } 
    public string EndedAt { get; set; } 
    public Player Receiver { get; set; }
    public Player Requester { get; set; }
    public List<Item> ReceiverItems { get; set; }
    public List<Item> RequesterItems { get; set; }
}