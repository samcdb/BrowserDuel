using BrowserDuel.Events;
using BrowserDuel.Interfaces;
using BrowserDuel.Models;
using Microsoft.AspNetCore.SignalR;

namespace BrowserDuel.Services
{
    public class MatchMakingService : IMatchMakingService
    {
        // can't use regular queue as some players may be skipped over if elo does not match
        // order matters - older items must be prioritised
        IList<Player> _matchQueue = new List<Player>();
        public event EventHandler<MatchFoundEventArgs> MatchFound;

        public void AddToQueue(Player player)
        {
            for (int i = 0; i < _matchQueue.Count; i++)
            {
                // TODO: expand on elo match making
                Player queuedPlayer = _matchQueue[i];
                // TODO:  make elo window that widens based on time spent in queue
                // check if player elo overlaps with queuedPlayer elo window
                if (queuedPlayer.Elo - 200 < player.Elo && player.Elo < queuedPlayer.Elo + 200)
                {
                    // make match and trigger event
                    // TODO: Make this thread safe, lock?
                    _matchQueue.RemoveAt(i);
                    Match match = new Match(new Player[] { player, queuedPlayer });
                    MatchFound?.Invoke(this, new MatchFoundEventArgs(match));
                    return;
                }
            }

            // failed to immediately find match
            _matchQueue.Add(player);
        }

        public void RemoveFromQueue(string connectionId)
        {
            throw new NotImplementedException();
        }
    }
}
