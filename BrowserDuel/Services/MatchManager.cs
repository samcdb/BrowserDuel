using BrowserDuel.Interfaces;
using BrowserDuel.Events;
using Microsoft.AspNetCore.SignalR;
using BrowserDuel.Hubs;
using BrowserDuel.Models;
using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Services
{
    // rules: anything sent to the client must be from a player perspective, don't make the client interpret general match data
    // only talk to clients via group
    public class MatchManager : IMatchManager
    {
        private readonly IMatchMakingService _matchMakingService;
        private readonly IHubContext<MatchHub, IMatchClient> _matchHubContext;
        private Dictionary<Guid, Match> _activeMatchCache;

        public MatchManager(IMatchMakingService matchMakingService, IHubContext<MatchHub, IMatchClient> matchHubContext)
        {
            _matchMakingService = matchMakingService;
            _matchHubContext = matchHubContext;
            _matchMakingService.MatchFound += MatchFoundEventHandler;

            _activeMatchCache = new Dictionary<Guid, Match>();
        }

        public Task ProcessReactionClickResult(string connectionId, long timeTaken)
        {
            throw new NotImplementedException();
        }

        private async void MatchFoundEventHandler(object sender, MatchFoundEventArgs e)
        {
            Match newMatch = e.NewMatch;
            string groupId = newMatch.Id.ToString();
            // create group for players
            await Task.WhenAll(newMatch.Players.Values
                .Select(player => _matchHubContext.Groups.AddToGroupAsync(player.ConnectionId, groupId)));
            // send matchFound to players
            // send enemy name to each player
            await Task.WhenAll(newMatch.Players.Values
                .Select(player => _matchHubContext.Clients.GroupExcept(groupId, new string[] { player.ConnectionId })
                    .MatchFound(new MatchFoundDto { Id = newMatch.Id.ToString(), EnemyName =  newMatch.GetOtherPlayer(player.ConnectionId).Name}))
                );
            _activeMatchCache[newMatch.Id] = newMatch;
        }
    }
}
