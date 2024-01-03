using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Databaseaccess.Models
{
    public class Inventory
    {
        public int WeightLimit { get; set; }
        public int Dimensions { get; set; }
        public int FreeSpots { get; set; }
        public int UsedSpots { get; set; }
        [JsonIgnore]
        public List<Item> Items { get; set; }
    }
}
