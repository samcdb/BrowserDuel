using BrowserDuel.Events;
using BrowserDuel.Models;

namespace BrowserDuel.Interfaces
{
    public interface IMatchMakingService
    {
        void AddToQueue(Player player);
        void RemoveFromQueue(string connectionId);
        event EventHandler<MatchFoundEventArgs> MatchFound;
    }
}
