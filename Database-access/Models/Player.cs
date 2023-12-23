using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databaseaccess.Models
{
    public class Player
    {
        // CREATE (n:Player {name: "Dianne", email: "dia@themail.com", bio: "This looks fun", achievementPoints:22, createdAt:"23-12-2023", password:"hHJgYzI26pIaO", gold: 17, honor:23});
        // public String id { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        public String bio { get; set; }
        public int achievementPoints { get; set; }
        public String createdAt { get; set; }
        public String password { get; set; }
        public int gold { get; set; }
        public int honor { get; set; }
    }
}
