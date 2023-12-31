using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Databaseaccess.Models
{
    public class PlayerFight
    {
        public string Winner { get; set; }
        public int Experience { get; set; }
        public int Honor { get; set; }
        [JsonIgnore]
        public Player Player1 { get; set; }
        [JsonIgnore]
        public Player Player2 { get; set; }
    }
}
