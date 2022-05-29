namespace BrowserDuel.Interfaces
{
    // get match via index
    // 
    public interface IMatchManager
    {
        Task SetPlayerReady(string matchId, string connectionId);
        Task ProcessReactionClickResult(string connectionId, int timeTaken);
    }
}
