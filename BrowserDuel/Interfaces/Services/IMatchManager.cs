namespace BrowserDuel.Interfaces
{
    // get match via index
    // 
    public interface IMatchManager
    {
        Task SetPlayerReady(string matchId, string connectionId);
        Task ProcessReactionClickAction(string matchId, string connectionId, int timeTaken);
        Task ProcessAimAction(string matchId, string connectionId, int? timeTaken);
    }
}
