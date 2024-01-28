public class MonsterBattleCreateDto
{
    public string MonsterName { get; set; }
    public string PlayerName { get; set; }

}
public class MonsterBattleUpdateDto
{
    public int MonsterBattleId { get; set; }
    public string[] LootItemsNames { get; set; }
}
