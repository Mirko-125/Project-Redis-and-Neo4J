using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Equipment
    {
        public double AverageQuality { get; set; }
        public double Weight { get; set; } 
        public List<Gear> EquippedGear { get; set; }
        public Equipment(INode node)
        {
            AverageQuality = node["averageQuality"].As<double>();
            Weight = node["weight"].As<double>();
            EquippedGear = new List<Gear>();
        }
    }
}
