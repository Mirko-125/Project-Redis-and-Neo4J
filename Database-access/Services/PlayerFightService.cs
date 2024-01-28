using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class PlayerFightService
    {
        private readonly IDriver _driver;
        public readonly string type = "PlayerFight";
        public readonly string _key = "playerFight";

        public static PlayerFight BuildPlayerFight(IRecord record)
        {
            var playerFightNode = record["playerFight"].As<INode>();
            var players = record["players"].As<List<INode>>();
            var player1Node = players[0];
            var player2Node = players[1];
            return new(playerFightNode, player1Node, player2Node);
        }

        public PlayerFightService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IResultCursor> CreateAsync(PlayerFightCreateDto playerFight)
        {
            var session = _driver.AsyncSession();
            if(!await PlayerService.PlayerExist(playerFight.Player1Name, session))
            {
                throw new Exception("Player1 with this name doesn't exist.");
            }
            if(!await PlayerService.PlayerExist(playerFight.Player2Name, session))
            {
                throw new Exception("Player2 with this name doesn't exist.");
            }
            if(playerFight.Player1Name == playerFight.Player2Name)
            {
                throw new Exception("Player can't fight himself.");
            }
            if(playerFight.Winner != playerFight.Player1Name && playerFight.Winner != playerFight.Player2Name)
            {
                throw new Exception("Winner must be one of the players.");
            }
            var parameters = new
            {
                winner = playerFight.Winner,
                experience = playerFight.Experience,
                honor = playerFight.Honor,
                player1name = playerFight.Player1Name,
                player2name = playerFight.Player2Name
            };
            string query = $@"
                CREATE ({_key}:{type} {{
                    winner: $winner,
                    experience: $experience,
                    honor: $honor
                }})
                WITH {_key}
                MATCH (n1:Player) WHERE n1.name=$player1name
                MATCH (n2:Player) WHERE n2.name=$player2name
                CREATE (n1)<-[:PARTICIPATING_PLAYERS]-({_key})-[:PARTICIPATING_PLAYERS]->(n2) ";
            var result = await session.RunAsync(query, parameters);
            return result;
        }
       
        public async Task<IResultCursor> UpdateAsync(PlayerFightUpdateDto playerFight)
        {
            var session = _driver.AsyncSession();
            if(!await PlayerFightExist(playerFight.PlayerFightId, session))
            {
                throw new Exception("Player Fight with this ID doesn't exist.");
            }
            var parameters = new 
            { 
                playerFightid = playerFight.PlayerFightId,
                winner = playerFight.Winner,
                experience = playerFight.Experience,
                honor = playerFight.Honor 
            };
            var query = $@"
                MATCH ({_key}:{type}) WHERE Id({_key})=$playerFightid
                    SET {_key}.winner= $winner
                    SET {_key}.experience= $experience
                    SET {_key}.honor= $honor
                RETURN {_key}";
            var result= await session.RunAsync(query, parameters);
            return result;
        }
        public async Task<List<PlayerFight>> GetAllAsync()
        {
            var session = _driver.AsyncSession();  
            var query = $@"
                MATCH ({_key}:{type})-[:PARTICIPATING_PLAYERS]->(p:Player)
                RETURN {_key} , COLLECT(p) AS players";
            var cursor = await session.RunAsync(query);
            var playerFights = new List<PlayerFight>();
            await cursor.ForEachAsync(record =>
            {
                playerFights.Add(BuildPlayerFight(record));
            });
            return playerFights;
        }
       public async Task<PlayerFight> GetOneAsync(int playerFightId)
        {
            var session = _driver.AsyncSession();
            if(!await PlayerFightExist(playerFightId, session))
            {
                throw new Exception("Player Fight with this ID doesn't exist.");
            }
            var query = $@"
                MATCH ({_key}:{type})-[:PARTICIPATING_PLAYERS]->(p:Player)
                    WHERE Id({_key})= $playerFightId
                RETURN {_key}, COLLECT(p) AS players";
            var cursor = await session.RunAsync(query, new{playerFightId});          
            return BuildPlayerFight(await cursor.SingleAsync());
        } 

         public async Task<IResultCursor> DeleteAsync(int playerFightId)
        {
            var session = _driver.AsyncSession();
            if(!await PlayerFightExist(playerFightId, session))
            {
                throw new Exception("Player Fight with this ID doesn't exist.");
            }
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE ID({_key})=$playerFightId
                DETACH DELETE {_key}";
            return await session.RunAsync(query, new{playerFightId});
        }

         public static async Task<bool> PlayerFightExist(int playerFightId, IAsyncSession sessions)
        {
            var session = sessions;
            string query= $@" 
                MATCH (n:PlayerFight) 
                    WHERE ID(n)=$playerFightId 
                RETURN COUNT(n) AS count";
            var cursor = await session.RunAsync(query, new{playerFightId});
            var record = await cursor.SingleAsync();
            var br = record["count"].As<int>();
            if(br > 0)
            { 
                return true;
            }

            return false;
        }
    }
}