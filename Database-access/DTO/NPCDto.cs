public class NPCCreateDto
{
    public string Name { get; set; }
    public string Affinity { get; set; }
    public string ImageURL { get; set; }
    public string Zone{ get; set; }
    public string Mood { get; set; }
}
public class NPCUpdateDto
{
    public int NPCId { get; set; }
    public string Name { get; set; }
    public string Affinity { get; set; }
    public string ImageURL { get; set; }
    public string Zone{ get; set; }
    public string Mood { get; set; }
}