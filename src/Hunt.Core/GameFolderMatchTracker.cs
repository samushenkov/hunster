using Hunt.GameFolder.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Threading.Channels;

namespace Hunt.GameFolder
{
    public class GameFolderMatchTracker : IMatchProvider, IMatchEmitter
    {
        private readonly IOptions<GameFolderMatchTrackerOptions> _options;
        private readonly ILogger<GameFolderMatchTracker> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        private const string ATTRIBUTES_FILE = "attributes.xml";

        public GameFolderMatchTracker(IOptions<GameFolderMatchTrackerOptions> options, ILogger<GameFolderMatchTracker> logger)
        {
            _options = options;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(2, n => TimeSpan.FromSeconds(1));
        }

        public event Action<Match> OnMatchChanged;

        public async Task TrackAsync(CancellationToken token)
        {
            var options = _options.Value;

            if (options == null || 
                options.GameProfileFolder == null)
            {
                throw new InvalidOperationException("Configuration is missing.");
            }

            using var watcher = new FileSystemWatcher(options.GameProfileFolder, ATTRIBUTES_FILE)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            var changeEventChannel = CreateChannel();

            watcher.Changed += HandleFileChangeEvent;
            watcher.EnableRaisingEvents = true;

            while (true)
            {
                // Wait for change event
                await changeEventChannel.Reader.ReadAsync(token);

                _logger.LogInformation("File has been changed");

                // Wait for change event
                await changeEventChannel.Reader.SkipAsync(TimeSpan.FromSeconds(1), token);

                _logger.LogInformation("File has been changed. Getting the last match available");

                try
                {
                    var match = await GetLastMatchAsync(token);
                    var matchChangedCallback = OnMatchChanged;

                    _logger.LogInformation("Notifying subscribers");

                    if (matchChangedCallback != null)
                    {
                        matchChangedCallback(match);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException ocEx)
                    {
                        if (token == ocEx.CancellationToken)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                    }

                    _logger.LogError(ex, "Unable to publish update event");
                }
            }

            void HandleFileChangeEvent(object sender, FileSystemEventArgs evt)
            {
                // Notify file changed
                changeEventChannel.Writer.TryWrite(null);
            }
        }

        public async Task<Match> GetLastMatchAsync(CancellationToken token)
        {
            var options = _options.Value;

            if (options == null || 
                options.GameProfileFolder == null)
            {
                throw new InvalidOperationException("Configuration is missing.");
            }

            var attributesFilePath = Path.Combine(options.GameProfileFolder, ATTRIBUTES_FILE);

            try
            {
                async Task<Match> ParseAttributesAssync()
                {
                    using var attributesFile = File.Open(attributesFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    var missionBag = await GameFolderAttributesFileSerializer.DeserializeAsync(attributesFile, token);
                    var missionBagFileDate = File.GetLastWriteTimeUtc(attributesFile.SafeFileHandle);

                    return new Match
                    {
                        MissionBag = missionBag,
                        MissionVersion = $"{missionBagFileDate.Ticks}"
                    };
                }

                return await _retryPolicy.ExecuteAsync(ParseAttributesAssync);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to read game profile attributes.", ex);
            }
        }

        private static Channel<object> CreateChannel()
        {
            var channelOptions = new BoundedChannelOptions(1)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            };

            return Channel.CreateBounded<object>(channelOptions);
        }
    }
}