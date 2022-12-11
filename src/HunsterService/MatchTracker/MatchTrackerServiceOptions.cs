using Common.Configuration.CommandLine;

namespace HunsterService.MatchTracker
{
    public class MatchTrackerServiceOptions
    {
        [CommandLineAlias("--logMmr")]
        public bool LogMmr { get; set; }
    }
}