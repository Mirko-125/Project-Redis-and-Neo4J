using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Ability
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Cooldown { get; set; }
        public double Range { get; set; }
        public string Special { get; set; } 
        public int Heal { get; set; }
        public Ability(INode node)
        {
            Name = node["Name"].As<string>();
            Damage = node["Damage"].As<int>();
            Cooldown = node["Cooldown"].As<int>();
            Range = node["Range"].As<double>();
            Special = node["Special"].As<string>();
            Heal = node["Heal"].As<int>();
        }
    }
}
