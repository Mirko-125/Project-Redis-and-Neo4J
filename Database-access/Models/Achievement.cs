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
            Name = node["name"].As<string>();
            Type = node["type"].As<string>();
            Points = node["points"].As<int>();
            Conditions = node["conditions"].As<string>();
        }
    }
}
