namespace Hunt
{
    public interface IMatchEmitter
    {
        event Action<Match> OnMatchChanged;
    }
}