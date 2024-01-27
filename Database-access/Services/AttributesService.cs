using Cache;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Neo4j.Driver;
using Databaseaccess.Models;

namespace Services{
    public class AttributesService
    {
        public static string CreateAttributes( AttributesDto attributesDto)
        {
            double strength = attributesDto.Strength;
            double agility = attributesDto.Agility;
            double intelligence= attributesDto.Intelligence;
            double stamina=attributesDto.Stamina;
            double faith=attributesDto.Faith;
            double experience=attributesDto.Experience;
            int level=attributesDto.Level;
            string query = $@"
                CREATE (attributes:Attributes {{ 
                        strength: {strength}, 
                        agility: {agility}, 
                        intelligence: {intelligence}, 
                        stamina: {stamina}, 
                        faith: {faith}, 
                        experience: {experience}, 
                        level: {level}
                        }})
                ";

            return query;
        }
         public static string UpdateAttributes( AttributesDto attributesDto)
        {
            double strength = attributesDto.Strength;
            double agility = attributesDto.Agility;
            double intelligence= attributesDto.Intelligence;
            double stamina=attributesDto.Stamina;
            double faith=attributesDto.Faith;
            double experience=attributesDto.Experience;
            int level=attributesDto.Level;
            string query = $@"
                SET m.strength= {strength}
                SET m.agility= {agility}
                SET m.intelligence= {intelligence}
                SET m.stamina= {stamina}
                SET m.faith= {faith}
                SET m.experience= {experience}
                SET m.level= {level}
                ";
            return query;
        }
    }
}