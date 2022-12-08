using Common.Configuration.CommandLine;

namespace Hunt.GameFolder
{
    public class GameFolderMatchTrackerOptions
    {
        [CommandLineAlias("--profileDir")]
        public string GameProfileFolder { get; set; }
    }
}