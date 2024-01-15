using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Achievement
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public string Conditions { get; set; }
        public Achievement(INode node)
        {
            Name = node["Name"].As<string>();
            Type = node["Type"].As<string>();
            Points = node["Points"].As<int>();
            Conditions = node["Conditions"].As<string>();
        }
    }
}
