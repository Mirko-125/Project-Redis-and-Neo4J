public class MonsterBattleCreateDto
{
    //vreme
    //string StartedAt { get; set; }
    //public bool IsFinalized { get; set; }//false
    //public string EndedAt{ get; set; }
    public int MonsterId { get; set; }
    public int PlayerId { get; set; }
    //public string[] LootItemsNames { get; set; }
}
public class MonsterBattleUpdateDto
{
    public int MonsterBattleId { get; set; }
    //public string EndedAt { get; set; }
    //public bool IsFinalized { get; set; }//true
    public string[] LootItemsNames { get; set; }
}
