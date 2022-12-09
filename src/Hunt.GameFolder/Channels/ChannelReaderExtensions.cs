using System.Threading.Channels;

namespace Hunt.GameFolder.Channels
{
    public static class ChannelReaderExtensions
    {
        public static async Task SkipAsync<T>(this ChannelReader<T> channelReader, TimeSpan timeout, CancellationToken token)
        {
            // Apply some throttling
            await Task.Delay(timeout, token);

            // Meke sure there are no more pending updates
            while (true)
            {
                var itemExists = channelReader.TryRead(out var _);

                if (itemExists == false)
                {
                    break;
                }
            }
        }
    }
}
