public class ItemDto
{
    public string Name { get; set; } 
    public string Type { get; set; }
    public double Weight { get; set; } 
    public int Dimensions { get; set; }
    public int Value { get; set; }

}
public class GearDto : ItemDto
{
    public int Slot { get; set; }
    public int Level { get; set; }
    public string Quality { get; set; }
    public AttributesDto Attributes { get; set; } 
}
public class ConsumableDto : ItemDto
{
    public string Effect { get; set; }

}
