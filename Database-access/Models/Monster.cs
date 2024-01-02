using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
