using Hunt.GameFolder;

namespace HunsterService.MatchTracker
{
    internal class MatchTrackerService : BackgroundService
    {
        private readonly GameFolderMatchTracker _matchTracker;

        public MatchTrackerService(GameFolderMatchTracker matchTracker)
        {
            _matchTracker = matchTracker;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _matchTracker.TrackAsync(stoppingToken);
        }
    }
}