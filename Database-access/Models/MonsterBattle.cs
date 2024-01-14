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
        public MonsterBattle(INode node)
        {
            StartedAt = node["StartedAt"].As<string>();
            EndedAt = node["EndedAt"].As<string>();
            IsFinalized = node["IsFinalized"].As<bool>();
            Monster = new Monster(node);
            Player = new Player(node);
            Loot = new List<Item>();
        }
    }
}
