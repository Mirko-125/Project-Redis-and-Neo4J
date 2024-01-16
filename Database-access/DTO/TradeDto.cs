public class TradeCreateDto 
{
    public bool IsFinalized { get; set; } 
    public int ReceiverGold { get; set; } 
    public int RequesterGold { get; set; } 
    public string StartedAt { get; set; } 
    public string EndedAt { get; set; } 
    public int ReceiverID { get; set; }
    public int RequesterID { get; set; }
    public string[] ReceiverItemNames { get; set; }
    public string[] RequesterItemNames { get; set; }
}

public class TradeUpdateDto 
{
    public int TradeID { get; set; }
    public bool IsFinalized { get; set; } 
    public int ReceiverGold { get; set; } 
    public int RequesterGold { get; set; } 
   
}