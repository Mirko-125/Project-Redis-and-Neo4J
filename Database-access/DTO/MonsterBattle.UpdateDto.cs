public class MonsterBattleUpdateDto
{
    public int MonsterBattleId { get; set; }
    public string EndedAt { get; set; }
    //public bool IsFinalized { get; set; }//true
    public string[] LootItemsNames { get; set; }
}