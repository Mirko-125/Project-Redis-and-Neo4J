public class AbilityDTO
{
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Cooldown { get; set; }
        public double Range { get; set; }
        public string Special { get; set; } 
        public int Heal { get; set; }
}

public class UpdateAbilityDto : AbilityDTO{
        public string OldName { get; set; }
}