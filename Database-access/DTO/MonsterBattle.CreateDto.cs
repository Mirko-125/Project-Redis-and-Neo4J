public class MonsterBattleCreateDto
{
    //vreme
    public string StartedAt { get; set; }
    //public bool IsFinalized { get; set; }//false
    public string EndedAt{ get; set; }
    public int MonsterId { get; set; }
    public int PlayerId { get; set; }
    //public string[] LootItemsNames { get; set; }
}
