public class TradeCreateDto 
{ 
    public int ReceiverGold { get; set; } 
    public int RequesterGold { get; set; } 
    public string ReceiverName { get; set; }
    public string RequesterName { get; set; }
    public string[] ReceiverItemNames { get; set; }
    public string[] RequesterItemNames { get; set; }
}

public class TradeUpdateDto 
{
    public int TradeID { get; set; }
    public int ReceiverGold { get; set; } 
    public int RequesterGold { get; set; } 
   
}