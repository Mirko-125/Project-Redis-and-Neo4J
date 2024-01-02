using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Achievement
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public string Conditions { get; set; }
    }
}
