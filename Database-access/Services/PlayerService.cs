using Cache;
using Databaseaccess.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using ServiceStack;

namespace Services
{
    public class PlayerService
    {
        private readonly IDriver _driver;
        public PlayerService(IDriver driver)
        {
            _driver = driver;
        }

        public static Player BuildSimplePlayer(IRecord record)
        {
            var player = record["n"].As<INode>();
            return new Player(player);
        }

        public static Player BuildComplexPlayer(IRecord record)
        {
            var playerNode = record["n"].As<INode>();
            var inventory = record["inventory"].As<INode>();
            var achievements = record["achievements"].As<List<INode>>();
            var abilities = record["abilities"].As<List<INode>>();
            var equipment = record["equipment"].As<INode>();
            var attributes = record["attributes"].As<INode>();
            var equippedItems = record["equippedItems"].As<List<Dictionary<string, INode>>>();
            var inventoryItems = record["inventoryItems"].As<List<Dictionary<string, INode>>>();
            Player player = new(playerNode, inventory, equipment, attributes, achievements, abilities, inventoryItems, equippedItems);
            return player;
        }

        public async Task<IResultCursor> CreateAsync(PlayerDto player)
        {
            
            var session = _driver.AsyncSession();

            if (await PlayerExist(player.Name, session))
            {
                throw new Exception("This name is already taken");
            }

            var parameters = new
            {
                name = player.Name,
                email = player.Email,
                bio = player.Bio,
                createdAt = DateTime.Now,
                password = player.Password,
                cls = player.Class
            };

            var query = @"
                CREATE (n:Player { 
                    name: $name, 
                    email: $email, 
                    bio: $bio, 
                    achievementPoints: 0, 
                    createdAt: $createdAt, 
                    password: $password,
                    gold: 0, 
                    honor: 0
                })

                WITH n
                    MATCH (class:Class) WHERE class.name = $cls
                    CREATE (n)-[:IS]->(class)

                WITH n, class
                    MATCH (class)-[:HAS_BASE_ATTRIBUTES]->(x)
                    CREATE (m:Inventory {
                        weightLimit : 0, 
                        dimensions: 0, 
                        freeSpots: 0, 
                        usedSpots: 0
                    })"
                    + AttributeQueryBuilder.CreateAttributes("o", "x.")
                    +
                    @" 
                    CREATE (p:Equipment {
                        averageQuality: 0, 
                        weight: 0
                    })
                    CREATE (n)-[:OWNS]->(m)
                    CREATE (n)-[:HAS]->(o)
                    CREATE (n)-[:WEARS]->(p)";

            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, parameters);
            return cursor;
            
        }

        public async Task<List<Player>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var query = "MATCH (n:Player) return n";
            var cursor = await session.RunAsync(query);   
            List<Player> players = [];
            await cursor.ForEachAsync(record =>
            {
                players.Add(BuildSimplePlayer(record));
            });

            return players;
        }

        public async Task<IResultCursor> LevelUpAsync(string playerName)
        {
            var session = _driver.AsyncSession();
            string attributeIdentifier = "attributes";
            string levelGainAttributes = "levelGainAttributes";
            var query = $@"
                MATCH (player:Player)
                    WHERE player.name = $playerName
                MATCH (player)-[:IS]->(class)
                MATCH (class)-[:LEVEL_GAINS_ATTRIBUTES]->({levelGainAttributes})
                MATCH (player)-[:HAS]->({attributeIdentifier})
                SET {attributeIdentifier}.experience = {attributeIdentifier}.experience - 1000 ";
            query += AttributeQueryBuilder.AttributeAddition(attributeIdentifier, levelGainAttributes + ".");

            var result = await session.RunAsync(query, new {playerName});
            return result;
        }

        public async Task<IResultCursor> AddItemAsync(string itemName, string playerName)
        {
            var session = _driver.AsyncSession();
            string findItemQuery = ItemQueryBuilder.FindItem(ItemQueryBuilder.singularKey, itemName);
            string findPlayerQuery = $@" 
                WITH {ItemQueryBuilder.singularKey}
                MATCH (player:Player)-[:OWNS]->(inventory:Inventory)
                    WHERE player.name = $playerName
                    
                CREATE (inventory)-[:CONTAINS]->(item)";
            
            string query = findItemQuery + findPlayerQuery;
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, new {playerName});
            return cursor;
        }
        public async Task<Player> GetPlayerAsync(string name)
        {
            var session = _driver.AsyncSession();
            string query = @"
                MATCH (n:Player) WHERE n.name = $name
                MATCH (n)-[:HAS]->(attributes)
                WITH n, attributes
                    OPTIONAL MATCH (n)-[:ACHIEVED]->(achievement)
                WITH n, attributes, COLLECT(achievement) as achievements
                    OPTIONAL MATCH (n)-[:KNOWS]->(ability)
                WITH n, attributes, achievements, COLLECT(ability) AS abilities

                MATCH (n)-[:OWNS]->(inventory)
                        OPTIONAL MATCH (inventory)-[:CONTAINS]->(inventoryItem:Item)
                                OPTIONAL MATCH (inventoryItem)-[:HAS]->(inventoryA:Attributes)
                WITH n, attributes, achievements, abilities, inventory,
                COLLECT({
                    item: inventoryItem,
                    attributes: CASE WHEN inventoryItem:Gear THEN inventoryA ELSE NULL END
                }) AS inventoryItems

                MATCH (n)-[:IS]->(class)
                MATCH (n)-[:WEARS]->(equipment)
                        OPTIONAL MATCH (equipment)-[:CONTAINS]->(equippedItem:Item)
                                OPTIONAL MATCH (equippedItem)-[:HAS]->(equipmentA:Attributes)
                WITH n, attributes, achievements, abilities, inventory, class, equipment, inventoryItems,
                COLLECT(DISTINCT{
                    item: equippedItem,
                    attributes: CASE WHEN equippedItem:Gear THEN equipmentA ELSE NULL END
                }) AS equippedItems
            return n, attributes, achievements, abilities, inventory, class, equipment, inventoryItems, equippedItems";
            Console.WriteLine(query);
            var cursor = await session.RunAsync(query, new {name});  
            var record = await cursor.SingleAsync();
            Player player = BuildComplexPlayer(record);
            return player;
        }

        public async Task<IResultCursor> DeleteAsync(string name)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (p:Player) WHERE p.name = $name
                DETACH DELETE p";
            var result = await session.RunAsync(query, new {name});
            return result;
        }

        public async Task<IResultCursor> AddExperience(string name, double experience)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (p:Player)-[:HAS]->(a:Attributes)
                    WHERE p.name = $name
                SET a.experience = a.experience + $experience 
                RETURN a.experience AS currentExperience";
            var cursor = await session.RunAsync(query, new {name, experience});
            var record = await cursor.SingleAsync();
            double currentExperience = record["currentExperience"].As<double>();
            if(currentExperience > 1000)
            {
                cursor = await LevelUpAsync(name);
            }
            return cursor;
        }
        public async Task<IResultCursor> AddHonor(string name, int honor)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (p:Player)
                    WHERE p.name = $name
                SET p.honor = p.honor + $honor 
                ";
            var result = await session.RunAsync(query, new {name, honor});
            return result;
        }

        public async Task<IResultCursor> UnequipGear(string gearName, string playerName)
        {
            var session = _driver.AsyncSession();
            var parameters = new {playerName, gearName};
            var query = @$"
                MATCH (player:Player)-[:WEARS]->(equipment:Equipment) 
                    WHERE player.name = $playerName
                MATCH (equipment)-[relation:CONTAINS]->(gear:Gear)
                    WHERE gear.name = $gearName
                SET equipment.weight= equipment.weight - gear.weight
                DELETE relation";
            return await session.RunAsync(query, parameters);
        }

        public async Task<IResultCursor> EquipGear(string gearName, string playerName)
        {
            var session = _driver.AsyncSession();
            var parameters = new {playerName, gearName};
            var query = @$"
                MATCH (player:Player)-[:WEARS]->(equipment:Equipment) 
                    WHERE player.name = $playerName
                MATCH (gear:Gear)
                    WHERE gear.name=$gearName
                SET equipment.weight = equipment.weight + gear.weight
                WITH equipment, gear
                CREATE (equipment)-[:CONTAINS]->(gear)";
            return await session.RunAsync(query, parameters);
        }
         public static async Task<bool> PlayerExist(string name, IAsyncSession sessions)
        {
            var session = sessions;
            string query= $@" 
                MATCH (player:Player) 
                    WHERE player.name=$name 
                RETURN COUNT(player) AS count";
            var cursor = await session.RunAsync(query, new{name});
            var record = await cursor.SingleAsync();
            var br = record["count"].As<int>();
            if(br > 0)
            { 
                return true;
            }

            return false;
        }

        public async Task<IResultCursor> RemoveItem(string itemName, string playerName)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (player:Player)-[:OWNS]->(inventory:Inventory) 
                    WHERE player.name = $playerName
                MATCH (inventory)-[relation:CONTAINS]->(item:Item)
                    WHERE item.name = $itemName
                DELETE relation
                ";
            var parameters = new { itemName, playerName };
            Console.WriteLine(query);
            return await session.RunAsync(query, parameters);
        }
        public async Task<IResultCursor> AddGold(string name, int gold)
        {
            var session = _driver.AsyncSession();
            var query = $@"
                MATCH (p:Player)
                    WHERE p.name = $name
                SET p.gold = p.gold + $gold 
                ";
            var result = await session.RunAsync(query, new {name, gold});
            return result;
        }

    }
}