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

    public Trade(INode tradeNode, INode playerRec, INode playerReq, List<Dictionary<string, INode>> itemsRecNodeList, List<Dictionary<string, INode>> itemsReqNodeList)
    {

        ReceiverItems = [];
        if (itemsRecNodeList.Count > 0)
        {
            itemsRecNodeList.ForEach(itemAndAttributes => 
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
                ReceiverItems.Add(item);
            });
        }

        RequesterItems = [];
        if (itemsReqNodeList.Count > 0)
        {
            itemsReqNodeList.ForEach(itemAndAttributes => 
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
                RequesterItems.Add(item);
            });
        }
        IsFinalized = tradeNode["isFinalized"].As<bool>();
        ReceiverGold = tradeNode["receiverGold"].As<int>();
        RequesterGold = tradeNode["requesterGold"].As<int>();
        StartedAt = tradeNode["startedAt"].As<string>();
        EndedAt = tradeNode["endedAt"].As<string>();
        Receiver = new Player(playerRec);
        Requester = new Player(playerReq);

    }
}