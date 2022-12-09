namespace Hunt.Entity
{
    public class Team
    {
        public int Handicap { get; set; }
        public bool IsInvite { get; set; }
        public int Mmr { get; set; }
        public bool OwnTeam { get; set; }
        public IEnumerable<Player> Players { get; set; }
    }
}