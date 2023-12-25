using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Ability
    {
        public string Name { get; set; }
        public double Damage { get; set; }
        public int Cooldown { get; set; }
        public double Range { get; set; }
        public string Special { get; set; } // nisam siguran za tip
        public double Heal { get; set; } // takodje tip
        public Player Player { get; set; }
        public List<Class> Classes { get; set; } // ILI JEDNA
    }
}
