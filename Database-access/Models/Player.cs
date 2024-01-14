using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class Player
    {
        // CREATE (n:Player {name: "Dianne", email: "dia@themail.com", bio: "This looks fun", achievementPoints:22, createdAt:"23-12-2023", password:"hHJgYzI26pIaO", gold: 17, honor:23});
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public int AchievementPoints { get; set; }
        public string CreatedAt { get; set; }
        public string Password { get; set; }
        public int Gold { get; set; }
        public int Honor { get; set; }
        public Attributes Attributes { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        [JsonIgnore]
        public List<Achievement> Achievements { get; set; }
        [JsonIgnore]
        public List<Ability> Abilities { get; set; }
        [JsonIgnore]
        public Class Class { get; set; }
        [JsonIgnore]
        public List<NPC> NPCs { get; set; } // Peđa

        public Player(INode node)
        {
            Name = node["Name"].As<string>();
            Email = node["Email"].As<string>();
            Bio = node["Bio"].As<string>();
            AchievementPoints = node["AchievementPoints"].As<int>();
            CreatedAt = node["CreatedAt"].As<string>();
            Password = node["Password"].As<string>();
            Gold = node["Gold"].As<int>();
            Honor = node["Honor"].As<int>();
            Attributes = new Attributes(node);
            Inventory = new Inventory(node);
            Equipment = new Equipment(node);
            Achievements = new List<Achievement>();
            Abilities = new List<Ability>();
            NPCs = new List<NPC>();
        }
    }
}
