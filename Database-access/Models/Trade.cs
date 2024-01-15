using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databaseaccess.Models;
using Neo4j.Driver;

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
    public Trade(INode node)
    {
        IsFinalized = node["IsFinalized"].As<bool>();
        ReceiverGold = node["ReceiverGold"].As<int>();
        RequesterGold = node["RequesterGold"].As<int>();
        StartedAt = node["StartedAt"].As<string>();
        EndedAt = node["EndedAt"].As<string>();
        Receiver = new Player(node);
        Requester = new Player(node);
        ReceiverItems = new List<Item>();
        RequesterItems = new List<Item>();
    }
}