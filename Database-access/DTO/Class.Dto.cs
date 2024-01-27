public class ClassDto
{
    public string Name { get; set; }
    public AttributesDto BaseAttributes { get; set; }
    public AttributesDto LevelGainAttributes { get; set; }
}

public class UpdateClassDto : ClassDto
{
    public string OldName { get; set; }
}



