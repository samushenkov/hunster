namespace Hunt.Entity
{
    public class MissionBag
    {
        public int FbeGoldBonus { get; set; }
        public int FbeHunterXpBonus { get; set; }

        public bool IsFbeBonusEnabled { get; set; }
        public bool IsHunterDead { get; set; }
        public bool IsQuickPlay { get; set; }

        public bool WasDeathlessUsed { get; set; }

        public bool Boss0 { get; set; }
        public bool Boss1 { get; set; }
        public bool Boss2 { get; set; }
        public bool Boss3 { get; set; }

        public IEnumerable<AccoladeEntry> Accolades { get; set; }
        public IEnumerable<Entry> Entries { get; set; }
        public IEnumerable<Team> Teams { get; set; }
    }
}