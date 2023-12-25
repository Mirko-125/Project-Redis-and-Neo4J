using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Inventory
    {
        public int WeightLimit { get; set; }
        public int Dimensions { get; set; }
        public int FreeSpots { get; set; }
        public int UsedSpots { get; set; }
        // Player
        public Player Player { get; set; }
        public List<Item> Items { get; set; }
    }
}
