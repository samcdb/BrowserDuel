using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Interfaces
{
    public interface IMatchClient
    {
        Task Connected(string message);
        Task MatchFound(MatchFoundDto match);
    }
}
