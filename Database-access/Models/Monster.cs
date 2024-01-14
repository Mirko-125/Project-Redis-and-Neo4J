using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Monster
    {
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Type{ get; set; }
        public string ImageURL { get; set; }
        public string Status { get; set; }
        public Attributes Attributes { get; set; }
        public List<Item> PossibleLoot { get; set; }
        public Monster(INode node)
        {
            Name = node["Name"].As<string>();
            Zone = node["Zone"].As<string>();
            Type = node["Type"].As<string>();
            ImageURL = node["ImageURL"].As<string>();
            Status = node["Status"].As<string>();
            Attributes = new Attributes(node);   
            PossibleLoot = new List<Item>();
        }
    }
}
