using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Equipment
    {
        public double AverageQuality { get; set; }
        public double Weight { get; set; } 
        public List<Gear> EquippedGear { get; set; }
    }
}
