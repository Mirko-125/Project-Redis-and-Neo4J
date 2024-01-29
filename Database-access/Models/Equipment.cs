using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Neo4j.Driver;
using Services;

namespace Databaseaccess.Models
{
    public class Equipment
    {
        public string AverageQuality { get; set; }
        public double Weight { get; set; } 
        public List<Item> EquippedGear { get; set; }

        private void SetBaseAttributes(INode node)
        {
            AverageQuality = node["averageQuality"].As<string>();
            Weight = node["weight"].As<double>();
        }
        public Equipment(INode node)
        {
            SetBaseAttributes(node);
            var items = node["items"].As<List<Dictionary<string, INode>>>();         
            EquippedGear = ItemQueryBuilder.BuildItemList(items);
        }

        public Equipment(INode node, List<Dictionary<string, INode>> equippedGear)
        {
            SetBaseAttributes(node);
            EquippedGear = ItemQueryBuilder.BuildItemList(equippedGear);
        }
    }
}
