namespace BrowserDuel.Interfaces
{
    // get match via index
    // 
    public interface IMatchManager
    {
        Task ProcessReactionClickResult(string connectionId, long timeTaken);
    }
}
