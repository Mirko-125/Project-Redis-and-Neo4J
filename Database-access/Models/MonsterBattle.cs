using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Databaseaccess.Models
{
    public class MonsterBattle
    {
        public string Status { get; set; }
         //vreme
        public string StartedAt { get; set; }
        //vreme
        public string EndedAt{ get; set; }
        public string Finalized { get; set; }
        //??????????
        [JsonIgnore]
        public Monster Monster { get; set; }
        //[JsonIgnore]
        //public Player Player { get; set; }
    }
}
