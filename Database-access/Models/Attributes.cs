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
        public double Inteligence { get; set; }
        public double Stanima { get; set; }
        public double Faith { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; } 

        public Attributes(INode node)
        {
            Strength = node["Strength"].As<double>();
            Agility = node["Agility"].As<double>();
            Inteligence = node["Inteligence"].As<double>();
            Stanima = node["Stanima"].As<double>();
            Faith = node["Faith"].As<double>();
            Experience = node["Experience"].As<double>();
            Level = node["Level"].As<int>();
        }
    }
}
