using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Attributes
    {
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public double Stamina { get; set; }
        public double Faith { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; } 

        public Attributes(INode node)
        {
            Strength = node["strength"].As<double>();
            Agility = node["agility"].As<double>();
            Intelligence = node["intelligence"].As<double>();
            Stamina = node["stamina"].As<double>();
            Faith = node["faith"].As<double>();
            Experience = node["experience"].As<double>();
            if (node.Properties.ContainsKey("level")){
                Level = node["level"].As<int>();
            }
        }
    }
}
