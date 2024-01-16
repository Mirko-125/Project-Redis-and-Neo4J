using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class MonsterBattle
    {
        //vreme
        public string StartedAt { get; set; }
        //vreme
        public string EndedAt{ get; set; }
        public bool IsFinalized { get; set; }
        public Monster Monster { get; set; }
        public Player Player { get; set; }
        public List<Item> Loot { get; set; }
        public MonsterBattle(INode monsterBattle, INode monster, INode monsterAttributes, INode player, List<Dictionary<string, INode>> possibleLootNodeList, List<Dictionary<string, INode>> lootNodeList=null)
        {
            Loot = [];
            if(lootNodeList != null){
                if (lootNodeList.Count > 0)
                {
                    lootNodeList.ForEach(itemAndAttributes => 
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
                        Loot.Add(item);
                    });
                }
            }
            StartedAt = monsterBattle["startedAt"].As<string>();
            EndedAt = monsterBattle["endedAt"].As<string>();
            IsFinalized = monsterBattle["isFinalized"].As<bool>();
            Monster = new Monster(monster, possibleLootNodeList, monsterAttributes);
            Player = new Player(player);
        }
    }
}
