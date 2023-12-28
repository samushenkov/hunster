using HunsterService.Cors;
using Hunt;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HunsterService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(CorsPolicies.HUNSTER_ENDPOINT)]
    public class HunsterController : ControllerBase
    {
        private readonly IMatchProvider _matchProvider;
        private readonly IMatchEmitter _matchEmitter;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public HunsterController(IMatchProvider missionBagProvider, IMatchEmitter matchEmitter, IHostApplicationLifetime applicationLifetime)
        {
            _matchProvider = missionBagProvider;
            _matchEmitter = matchEmitter;
            _applicationLifetime = applicationLifetime;
        }

        [HttpGet("{version}")]
        public async Task<Match> GetLastMatch(string version, CancellationToken token)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _applicationLifetime.ApplicationStopping);

            var matchSource = new TaskCompletionSource<Match>(TaskCreationOptions.RunContinuationsAsynchronously);
            var matchSourceUpdater = matchSource.SetResult;

            _matchEmitter.OnMatchChanged += matchSourceUpdater;

            try
            {
                var match = await _matchProvider.GetLastMatchAsync(tokenSource.Token);

                if (match.MissionVersion != version)
                {
                    // Return actual match
                    return match;
                }

                var matchTimeoutTask = Task.Delay(TimeSpan.FromMinutes(1), tokenSource.Token);
                var matchTask = await Task.WhenAny(matchTimeoutTask, matchSource.Task);

                if (matchTask != matchTimeoutTask)
                {
                    match = await matchSource.Task;
                }

                // Return actual match
                return match;
            }
            finally
            {
                _matchEmitter.OnMatchChanged -= matchSourceUpdater;
            }
        }
    }
}
