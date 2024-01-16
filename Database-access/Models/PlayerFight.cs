using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Neo4j.Driver;

namespace Databaseaccess.Models
{
    public class PlayerFight
    {
        public string Winner { get; set; }
        public int Experience { get; set; }
        public int Honor { get; set; }
        //[JsonIgnore]
        public Player Player1 { get; set; }
        //[JsonIgnore]
        public Player Player2 { get; set; }
        public PlayerFight(INode node, INode player1Node, INode player2Node)
        {
            Winner = node["winner"].As<string>();
            Experience = node["experience"].As<int>();
            Honor = node["honor"].As<int>();
            Player1 = new Player(player1Node);
            Player2 = new Player(player2Node);
        }
    }
}
