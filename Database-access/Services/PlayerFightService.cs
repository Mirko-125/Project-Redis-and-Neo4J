using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class PlayerFightService
    {
        private readonly IDriver _driver;
        public readonly string type = "PlayerFight";
        public readonly string _pluralKey = "playerFights";
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

            var parameters = new
            {
                winner = playerFight.Winner,
                experience = playerFight.Experience,
                honor = playerFight.Honor,
                playeri1 = playerFight.Player1Id,
                playeri2 = playerFight.Player2Id
            };
            string query = $@"
                        CREATE ({_key}:{type} {{
                            winner: $winner,
                            experience: $experience,
                            honor: $honor
                        }})
                        WITH {_key}
                        MATCH (n1:Player) WHERE id(n1)=$playeri1
                        MATCH (n2:Player) WHERE id(n2)=$playeri2
                        CREATE (n1)<-[:PARTICIPATING_PLAYERS]-({_key})-[:PARTICIPATING_PLAYERS]->(n2) ";
            var result = await session.RunAsync(query,parameters);
            return result;
        }
       
        public async Task<IResultCursor> UpdateAsync(PlayerFightUpdateDto playerFight)
        {
            var session = _driver.AsyncSession();
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
            var parameters = new { plFId = playerFightId };
            var query = $@"MATCH ({_key}:{type})-[:PARTICIPATING_PLAYERS]->(p:Player)
                            WHERE id({_key})= $plFId
                        RETURN {_key}, COLLECT(p) AS players";
            var cursor = await session.RunAsync(query,parameters);
                    
            return BuildPlayerFight(await cursor.SingleAsync());
        } 

         public async Task<IResultCursor> DeleteAsync(int playerFightId)
        {
            var session = _driver.AsyncSession();
            var parameters = new { plFight = playerFightId };
            var query = $@"
                MATCH ({_key}:{type}) WHERE ID({_key})=$plFight 
                DETACH DELETE {_key}";
            return await session.RunAsync(query, parameters);
        }
    }
}