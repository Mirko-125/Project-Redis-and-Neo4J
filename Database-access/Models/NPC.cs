using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class NPC
    {
        public string Name { get; set; }
        public string Affinity { get; set; }
        public string ImageURL { get; set; }
        public string Zone{ get; set; }
        public string Mood { get; set; }
        public NPC(INode node)
        {
            Name = node["Name"].As<string>();
            Affinity = node["Affinity"].As<string>();
            ImageURL = node["ImageURL"].As<string>();
            Zone = node["Zone"].As<string>();
            Mood = node["Mood"].As<string>();
        }
    }
}
