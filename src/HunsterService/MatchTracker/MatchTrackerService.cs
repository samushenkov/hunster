using Hunt;
using Hunt.GameFolder;
using Microsoft.Extensions.Options;

namespace HunsterService.MatchTracker
{
    internal class MatchTrackerService : BackgroundService
    {
        private readonly GameFolderMatchTracker _matchTracker;
        private readonly ILogger<MatchTrackerService> _logger;
        private readonly IOptions<MatchTrackerServiceOptions> _optionsAccessor;

        public MatchTrackerService(GameFolderMatchTracker matchTracker, ILogger<MatchTrackerService> logger, IOptions<MatchTrackerServiceOptions> optionsAccessor)
        {
            _matchTracker = matchTracker;
            _logger = logger;
            _optionsAccessor = optionsAccessor;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _matchTracker.OnMatchChanged += LogLastMatch;

            try
            {
                try
                {
                    var matchPrevious = await _matchTracker.GetLastMatchAsync(token);

                    if (matchPrevious != null)
                    {
                        LogLastMatch(matchPrevious);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to get last match");
                }

                // Listen for realtime udpates untill token is cancelled
                await _matchTracker.TrackAsync(token);
            }
            finally
            {
                _matchTracker.OnMatchChanged -= LogLastMatch;
            }
        }

        private void LogLastMatch(Match match)
        {
            var options = _optionsAccessor.Value;

            if (options == null || 
                options.LogMmr == false)
            {
                return;
            }

            var team = match.MissionBag.Teams.FirstOrDefault(t => t.OwnTeam);

            if (team != null)
            {
                var teamPlayer = team.Players.FirstOrDefault(p => p.IsPartner == false);

                if (teamPlayer != null)
                {
                    _logger.LogInformation("{username}: {mmr} mmr", 
                        teamPlayer.BloodLineName, 
                        teamPlayer.Mmr
                    );
                }
            }
        }
    }
}