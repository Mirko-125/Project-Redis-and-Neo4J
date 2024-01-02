using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Ability
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Cooldown { get; set; }
        public double Range { get; set; }
        public string Special { get; set; } 
        public int Heal { get; set; } 
        public Player Player { get; set; }
        public Class Class { get; set; } 
    }
}
