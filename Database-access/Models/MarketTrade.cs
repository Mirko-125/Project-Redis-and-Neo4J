using Databaseaccess.Models;

public class MarketTrade
{
    public int MarketGold { get; set; }
    public int PlayerGold { get; set; }
    public string Date { get; set; }
    public Player TradingPlayer { get; set; }
    public Marketplace TradedAt { get; set; }
    public List<Item> MarketItems { get; set; }
    public List<Item> PlayerItems { get; set; }
}