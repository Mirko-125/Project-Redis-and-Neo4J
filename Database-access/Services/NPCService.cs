using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;

namespace Services{
    public class NPCService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "NPC";
        public readonly string _key = "npc";

        public static NPC BuildNPC(IRecord record)
        {
            var npcNode = record["npc"].As<INode>();
            return new(npcNode);
        }

        public NPCService(IDriver driver, RedisCache cache)
        {
            _driver = driver;
            _cache = cache;
        }

        public async Task<IResultCursor> CreateAsync(NPCCreateDto npc)
        {
            var session = _driver.AsyncSession();
            if(await NPCExist(npc.Name, session))
            {
                throw new Exception("NPC already exists.");
            }
            var parameters = new
            {
                name = npc.Name,
                affinity = npc.Affinity,
                imageUrl= npc.ImageURL,
                zone = npc.Zone,                        
                mood=npc.Mood
            };
            var query = $@"
                CREATE ({_key}:{type} {{
                    name: $name,
                    affinity: $affinity,
                    imageURL: $imageUrl,
                    zone: $zone,
                    mood: $mood
                }})";
            var result = await session.RunAsync(query,parameters);
            return result;
        }
       
        public async Task<IResultCursor> UpdateAsync(NPCUpdateDto npc)
        {
            var session = _driver.AsyncSession();
            if(!await NPCExist(npc.Name, session))
            {
                throw new Exception("NPC with this name doesn't exist.");
            }
            var parameters = new 
            { 
                name=npc.Name,
                affinity=npc.Affinity,
                imageUrl=npc.ImageURL,
                zone=npc.Zone,
                mood=npc.Mood    
            };
            var query = $@"
                MATCH ({_key}:{type}) WHERE {_key}.name=$name
                    SET {_key}.name= $name
                    SET {_key}.affinity= $affinity
                    SET {_key}.imageURL= $imageUrl
                    SET {_key}.zone= $zone
                    SET {_key}.mood= $mood
                RETURN {_key}";             
            var result= await session.RunAsync(query, parameters);
            return result;
        }
    
        public async Task<NPC> Interaction(string playerName, string npcName)
        {
            var session = _driver.AsyncSession();
            if(!await NPCExist(npcName, session))
            {
                throw new Exception("NPC with this name doesn't exist.");
            }
            if(!await PlayerService.PlayerExist(playerName, session))
            {
                throw new Exception("Player with this name doesn't exist.");
            }
            var parameters = new 
            {   
                npcname=npcName,
                playername = playerName
            };
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE {_key}.name=$npcname 
                MATCH (n2:Player) 
                    WHERE n2.name=$playername
                MERGE (n2)-[rel:INTERACTS_WITH]->({_key})
                    SET rel.property_key = COALESCE(rel.property_key, 0) + 1
                RETURN rel.property_key AS incrementedProperty,{_key}"
            ;
            var cursor = await session.RunAsync(query,parameters);
            var record = await cursor.SingleAsync();
            var seq = record["incrementedProperty"].As<int>();
            var node = record["npc"].As<INode>();
            NPC npc=new(node);
            await _cache.SetDataAsync(npcName, npc, 100);
            return npc;
        }
        public async Task<List<NPC>> GetAllAsync()
        {
            var session = _driver.AsyncSession();
            var query = $"MATCH ({_key}:{type}) RETURN {_key}";
            var cursor = await session.RunAsync(query);
            var npcs = new List<NPC>();
            await cursor.ForEachAsync(record =>
            {
                npcs.Add(BuildNPC(record));
            });
            return npcs;
        }
        public async Task<NPC> GetOneAsync(string npcName)
        {
            var session = _driver.AsyncSession();
            if(!await NPCExist(npcName, session))
            {
                throw new Exception("NPC with this name doesn't exist.");
            }
            string key = npcName;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var npc = await _cache.GetDataAsync<NPC>(key);
                return npc;          
            }
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE {_key}.name=$npcName 
                RETURN {_key}";
            var cursor = await session.RunAsync(query, new{npcName});
            return BuildNPC(await cursor.SingleAsync());
        } 

        public async Task<IResultCursor> DeleteAsync(string npcName)
        {
            var session = _driver.AsyncSession();
            if(!await NPCExist(npcName, session))
            {
                throw new Exception("NPC with this name doesn't exist.");
            }
            string key = npcName;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                await _cache.DeleteAsync(key);         
            }
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE {_key}.name=$npcName
                DETACH DELETE {_key}";    
            return await session.RunAsync(query, new{npcName});
        }

        public static async Task<bool> NPCExist(string name, IAsyncSession sessions)
        {
            var session = sessions;
            string query= $@" 
                MATCH (npc:NPC) 
                    WHERE npc.name=$name 
                RETURN COUNT(npc) AS count";
            var cursor = await session.RunAsync(query, new{name});
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