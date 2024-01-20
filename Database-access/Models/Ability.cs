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
            Name = node["name"].As<string>();
            Damage = node["damage"].As<int>();
            Cooldown = node["cooldown"].As<int>();
            Range = node["range"].As<double>();
            Special = node["special"].As<string>();
            Heal = node["heal"].As<int>();
        }
    }
}
