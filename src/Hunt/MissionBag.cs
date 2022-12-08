namespace Hunt
{
    public class MissionBag
    {
        public bool IsHunterDead { get; set; }
        public bool IsQuickPlay { get; set; }

        public IEnumerable<AccoladeEntry> Accolades { get; set; }
        public IEnumerable<Entry> Entries { get; set; }
        public IEnumerable<Team> Teams { get; set; }
    }
}