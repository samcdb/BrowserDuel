using BrowserDuel.Interfaces;
using BrowserDuel.Events;
using Microsoft.AspNetCore.SignalR;
using BrowserDuel.Hubs;
using BrowserDuel.Models;
using BrowserDuel.Models.DataTransfer;
using BrowserDuel.Enums;
using BrowserDuel.Models.Games;

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

        public Task ProcessReactionClickResult(string connectionId, int timeTaken)
        {
            throw new NotImplementedException();
        }

        public async Task SetPlayerReady(string matchId, string connectionId)
        {
            Match match = _activeMatchCache[Guid.Parse(matchId)];

            // set player to be ready - if other player is ready then start next game
            bool bothPlayersReady = match.ReadyForNextGame(connectionId);

            if (bothPlayersReady)
            {
                Console.WriteLine("Starting next game");
                await StartNextGame(match);
            }
        }

        private async Task StartNextGame(Match match)
        {
            // tell match object to decide on next game and update its state
            match.NextGame();

            switch (match.CurrentGameType)
            {
                case GameType.ReactionClick:
                    ReactionClickGame currentGame = match.ReactionClickGame;
                    ReactionClickGameDto reactionClickGameDto = new ReactionClickGameDto
                    {
                        TimeUntilScreen = currentGame.TimeUntilScreen
                    };

                    await _matchHubContext.Clients.Group(match.Id.ToString())
                        .StartReactionClickGame(reactionClickGameDto);

                    break;
            }

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
