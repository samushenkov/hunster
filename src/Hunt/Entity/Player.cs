namespace Hunt.Entity
{
    public class Player
    {
        public string BloodLineName { get; set; }
        public int BountyExtracted { get; set; }
        public int BountyPickedUp { get; set; }
        public int DownedByMe { get; set; }
        public int DownedByTeammate { get; set; }
        public int DownedMe { get; set; }
        public int DownedTeammate { get; set; }
        public bool HadWellspring { get; set; }
        public bool HadBounty { get; set; }
        public bool IsPartner { get; set; }
        public bool IsSoulSurvivor { get; set; }
        public int KilledByMe { get; set; }
        public int KilledByTeammate { get; set; }
        public int KilledMe { get; set; }
        public int KilledTeammate { get; set; }
        public int Mmr { get; set; }
        public string ProfileId { get; set; }
        public bool Proximity { get; set; }
        public bool ProximityToMe { get; set; }
        public bool ProximityToTeammate { get; set; }
        public bool SkillBased { get; set; }
        public bool TeamExtraction { get; set; }
        public string TooltipDownedByTeammate { get; set; }
        public string TooltipBountyExtracted { get; set; }
        public string TooltipBountyPickedUp { get; set; }
        public string TooltipDownedByMe { get; set; }
        public string TooltipDownedMe { get; set; }
        public string TooltipDownedTeammate { get; set; }
        public string TooltipKilledByMe { get; set; }
        public string TooltipKilledByTeammate { get; set; }
        public string TooltipKilledMe { get; set; }
        public string TooltipKilledTeammate { get; set; }
    }
}