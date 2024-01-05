using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Databaseaccess.Models
{
    /* Ne funkcionise kako treba trenutno
    public enum ClassName
    {
        Emperor,
        Commander,
        Wizard,
        Archer,
        Spartan,
        Soldier,
        Assasin
    }
    */
    public class Class
    {
        public string Name { get; set; }
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
