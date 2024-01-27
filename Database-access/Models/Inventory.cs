using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Neo4j.Driver;
using Services;

namespace Databaseaccess.Models
{
    public class Inventory
    {
        public int WeightLimit { get; set; }
        public int Dimensions { get; set; }
        public int FreeSpots { get; set; }
        public int UsedSpots { get; set; }
        public List<Item> Items { get; set; }
        private void SetBaseAttributes(INode node)
        {
            WeightLimit = node["weightLimit"].As<int>();
            Dimensions = node["dimensions"].As<int>();
            FreeSpots = node["freeSpots"].As<int>();
            UsedSpots = node["usedSpots"].As<int>();
        }
        public Inventory(INode node)
        {
            SetBaseAttributes(node);
            var items = node["inventoryItems"].As<List<Dictionary<string, INode>>>();
            Items = ItemQueryBuilder.BuildItemList(items);
        }

        public Inventory(INode node, List<Dictionary<string, INode>> inventoryItems)
        {
            SetBaseAttributes(node);
            Items = ItemQueryBuilder.BuildItemList(inventoryItems);
        }
    }
}
