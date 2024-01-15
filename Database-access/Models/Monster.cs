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
        public Monster(INode monster, INode attributes)
        {
            Name = monster["name"].As<string>();
            Zone = monster["zone"].As<string>();
            Type = monster["type"].As<string>();
            ImageURL = monster["imageURL"].As<string>();
            Status = monster["status"].As<string>();
            Attributes = new Attributes(attributes);
            PossibleLoot = new List<Item>();
        }
    }
}
