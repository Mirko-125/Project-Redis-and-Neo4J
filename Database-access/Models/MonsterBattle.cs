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
        public MonsterBattle(INode monsterBattle, INode monster, INode monsterAttributes, INode player)
        {
            StartedAt = monsterBattle["StartedAt"].As<string>();
            EndedAt = monsterBattle["EndedAt"].As<string>();
            IsFinalized = monsterBattle["IsFinalized"].As<bool>();
            Monster = new Monster(monster, monsterAttributes);
            Player = new Player(player);
            Loot = new List<Item>();
        }
    }
}
