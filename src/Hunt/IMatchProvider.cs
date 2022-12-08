namespace Hunt
{
    public interface IMatchProvider
    {
        Task<Match> GetLastMatchAsync(CancellationToken token = default);
    }
}