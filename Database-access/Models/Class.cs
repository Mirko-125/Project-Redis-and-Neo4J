using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Databaseaccess.Models
{
    public enum ClassName
    {
        Emperor,
        Commander,
        Wizzard,
        Archer,
        Soldier,
        Assasin
    }
    public class Class
    {
        public ClassName Name { get; set; }
        [JsonIgnore]
        public List<Gear> Gear { get; set; }
        [JsonIgnore]
        public Attributes BaseAttributes { get; set; }
        [JsonIgnore]
        public Attributes LevelGainAttributes { get; set; }
        [JsonIgnore]
        public Ability Ability { get; set; }
    }
}
