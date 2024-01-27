public class AchievementDTO
{
        public string Name { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public string Conditions { get; set; }
}

public class UpdateAchievementDto : AchievementDTO
{
        public string OldName { get; set; }
}