public abstract class ItemCreateDto
{
    public string Name { get; set; } 
    public string Type { get; set; }
    public double Weight { get; set; } 
    public int Dimensions { get; set; }
    public int Value { get; set; }

}

public abstract class ItemUpdateDto
{
    public string Name { get; set; } 
    public string Type { get; set; } 
    public double Weight { get; set; } 
    public int Dimensions { get; set; }
    public int Value { get; set; }

}

public class GearCreateDto : ItemCreateDto
{
    public int Slot { get; set; }
    public string Quality { get; set; }
    public AttributesDto Attributes { get; set; } 
}

public class GearUpdateDto : ItemUpdateDto
{
    public int Slot { get; set; }
    public string Quality { get; set; }
    public AttributesDto Attributes { get; set; } 
}

public class ConsumableCreateDto : ItemCreateDto
{
    public string Effect { get; set; }

}

public class ConsumableUpdateDto : ItemUpdateDto
{
    public string Effect { get; set; }

}
