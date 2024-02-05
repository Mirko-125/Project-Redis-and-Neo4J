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
        public List<Achievement> Achievements { get; set; }
        public List<Ability> Abilities { get; set; }
        public Class Class { get; set; }
        public List<NPC> NPCs { get; set; } // Peđa

        private void BasePlayer(INode node)
        {
            Name = node["name"].As<string>();
            Email = node["email"].As<string>();
            Bio = node["bio"].As<string>();
            AchievementPoints = node["achievementPoints"].As<int>();
            CreatedAt = node["createdAt"].As<string>();
            Password = node["password"].As<string>();
            Gold = node["gold"].As<int>();
            Honor = node["honor"].As<int>();
        }

        public Player(INode node)
        {
            BasePlayer(node);
        }

        public Player(
            INode playerNode,
            INode inventoryNode,
            INode equipmentNode,
            INode attributes,
            List<INode> achievements,
            List<INode> abilities,
            List<Dictionary<string, INode>> inventoryItems,
            List<Dictionary<string, INode>> equippedItems)
        {
            BasePlayer(playerNode);
            Attributes = new Attributes(attributes);
            Inventory = new Inventory(inventoryNode, inventoryItems);
            Equipment = new Equipment(equipmentNode, equippedItems);
            Achievements = [];
            foreach (var node in achievements)
            {
                Achievements.Add(new Achievement(node));
            }
            Abilities = [];
            foreach (var node in abilities)
            {
                Abilities.Add(new Ability(node));
            }

            NPCs = [];
        }
    }
}
