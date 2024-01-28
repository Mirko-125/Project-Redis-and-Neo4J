
public class MonsterCreateDto
{
    public string Name { get; set; }
    public string Zone { get; set; }
    public string Type{ get; set; }
    public string ImageURL { get; set; }
    public string Status { get; set; }
    public AttributesDto Attributes { get; set; }
    public string[] PossibleLootNames { get; set; }
}

public class MonsterUpdateDto
{
    public string OldName { get; set; }
    public string Name { get; set; }
    public string Zone { get; set; }
    public string Type { get; set; }
    public string ImageURL { get; set; }
    public AttributesDto Attributes { get; set; }
    public string Status { get; set; }
}