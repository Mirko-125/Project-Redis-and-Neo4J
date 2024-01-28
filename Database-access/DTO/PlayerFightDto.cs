public class PlayerFightCreateDto
{
    public string Winner { get; set; }
    public double Experience { get; set; }
    public int Honor { get; set; }
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }
}
public class PlayerFightUpdateDto
{
    public int PlayerFightId { get; set; }
    public string Winner { get; set; }
    public double Experience { get; set; }
    public int Honor { get; set; }
}