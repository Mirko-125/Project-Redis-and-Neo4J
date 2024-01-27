using Neo4j.Driver;
using Databaseaccess.Models;
using Cache;

namespace Services{
    public class NPCService
    {
        private readonly IDriver _driver;
        private readonly RedisCache _cache;
        public readonly string type = "NPC";
        public readonly string _pluralKey = "npcs";
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
            var parameters = new 
            { 
                npcId = npc.NPCId,
                name=npc.Name,
                affinity=npc.Affinity,
                imageUrl=npc.ImageURL,
                zone=npc.Zone,
                mood=npc.Mood    
            };
            var query = $@"
                MATCH ({_key}:{type}) WHERE ID({_key})=$npcId
                    SET {_key}.name= $name
                    SET {_key}.affinity= $affinity
                    SET {_key}.imageURL= $imageUrl
                    SET {_key}.zone= $zone
                    SET {_key}.mood= $mood
                    RETURN {_key}";
                    
            var result= await session.RunAsync(query, parameters);
            return result;
        }
        //???????????????????????????????????
        public async Task<NPC> Interaction(int playerId, int npcId)
        {
            var session = _driver.AsyncSession();
            string key = _key + npcId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var npccached = await _cache.GetDataAsync<NPC>(key);
                return npccached;          
            }
            var parameters = new 
            {   npId=npcId,
                plId = playerId
            };
            var query = $@"
                MATCH ({_key}:{type}) WHERE id({_key})=$npId 
                MATCH (n2:Player) WHERE id(n2)=$plId
                MERGE (n2)-[rel:INTERACTS_WITH]->({_key})
                SET rel.property_key = COALESCE(rel.property_key, 0) + 1
                RETURN rel.property_key AS incrementedProperty,{_key}"
            ;
            var cursor = await session.RunAsync(query,parameters);
            var record = await cursor.SingleAsync();
            var seq = record["incrementedProperty"].As<int>();
            var node = record["npc"].As<INode>();

            NPC npc=new(node);
            
            await _cache.SetDataAsync(_key + npcId, npc, 25);

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
        public async Task<NPC> GetOneAsync(int npcId)
        {
            var session = _driver.AsyncSession();
            string key = _key + npcId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                var npc = await _cache.GetDataAsync<NPC>(key);
                return npc;          
            }
            var parameters = new { id = npcId };
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE Id({_key})=$id 
                RETURN {_key}";
            
            var cursor = await session.RunAsync(query,parameters);
            return BuildNPC(await cursor.SingleAsync());
        } 

        public async Task<IResultCursor> DeleteAsync(int npcId)
        {
            var session = _driver.AsyncSession();
            string key = _key + npcId;
            var keyExists = await _cache.CheckKeyAsync(key);
            if (keyExists)
            {
                await _cache.DeleteAsync(key);         
            }
            var parameters = new { id = npcId };
            var query = $@"
                MATCH ({_key}:{type}) 
                    WHERE Id({_key})=$id 
                DETACH DELETE {_key}";    
            return await session.RunAsync(query, parameters);
        }

    }
}