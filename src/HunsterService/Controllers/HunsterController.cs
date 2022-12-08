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

        public HunsterController(IMatchProvider missionBagProvider, IMatchEmitter matchEmitter)
        {
            _matchProvider = missionBagProvider;
            _matchEmitter = matchEmitter;
        }

        [HttpGet("{version}")]
        public async Task<Match> GetLastMatch(string version, CancellationToken token)
        {
            var matchSource = new TaskCompletionSource<Match>(TaskCreationOptions.RunContinuationsAsynchronously);
            var matchSourceUpdater = matchSource.SetResult;

            _matchEmitter.OnMatchChanged += matchSourceUpdater;

            try
            {
                var match = await _matchProvider.GetLastMatchAsync(token);

                if (match.MissionVersion != version)
                {
                    // Return actual match
                    return match;
                }

                var matchTimeoutTask = Task.Delay(TimeSpan.FromMinutes(1), token);
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
