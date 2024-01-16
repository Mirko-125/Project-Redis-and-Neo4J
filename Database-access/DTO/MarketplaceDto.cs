public class MarketplaceCreateDto
{
    public string Zone {get; set;}
    public int ItemCount {get; set;}
    public int RestockCycle{get; set;}

   
}


public class MarketplaceUpdateDto
{
    public int MarketplaceID { get; set; }
    public string Zone {get; set;}
    public int RestockCycle{get; set;}

}