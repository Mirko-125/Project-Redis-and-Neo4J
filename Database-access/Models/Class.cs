using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Class
    {
        public string Name { get; set; }
        // mozda treba da se dodaju instance (SIGURNO) mozda mozda
        public Player Player { get; set; }
        public List<Gear> Gear { get; set; } // not sure, come check this
        public Attributes BaseAttributes { get; set; }
        public Attributes LevelGainAttributes { get; set; }
        public Ability Ability { get; set; }
    }
}
