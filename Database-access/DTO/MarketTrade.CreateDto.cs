public class MarketTradeDto
{
    public int MarketGold { get; set; }
    public int PlayerGold { get; set; }
    public string Date { get; set; }
    public int PlayerID { get; set; }
    public int MarketplaceID { get; set; }
    public int[] PlayerItemsIDs { get; set; }
    public int[] MarketItemsIDs { get; set; }
}