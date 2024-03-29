﻿using BrowserDuel.Interfaces;
using BrowserDuel.Events;
using Microsoft.AspNetCore.SignalR;
using BrowserDuel.Hubs;
using BrowserDuel.Models;
using BrowserDuel.Models.Games;
using BrowserDuel.Models.DataTransfer;
using BrowserDuel.Enums;

namespace BrowserDuel.Services
{
    // rules: anything sent to the client must be from a player perspective, don't make the client interpret general match data
    // only talk to clients via group
    // can only request changes to match state via Match object - it holds the lock
    // cannot access the games stored by Match - Match handles those and returns what MatchManager needs, otherwise -
    // MatchManager could be given rights to set things it shouldn't be able to set - eg. ReactionClickGame.PlayerTimes
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

        private async void MatchFoundEventHandler(object sender, MatchFoundEventArgs e)
        {
            Match newMatch = e.NewMatch;
            string groupId = newMatch.Id.ToString();
            IEnumerable<Player> players = newMatch.Players.Values;
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
                // manager is given info about games - not the games themselves, to avoid having editing power
                case GameType.ReactionClick:
                    int timeUntilScreen = match.ReactionClickGameSetup;
                    ReactionClickGameDto reactionClickGameDto = new ReactionClickGameDto
                    {
                        TimeUntilScreen = timeUntilScreen
                    };
                    Console.WriteLine($"Sending start reaction click to match: {match.Id}");
                    await _matchHubContext.Clients.Group(match.Id.ToString())
                        .StartReactionClickGame(reactionClickGameDto);

                    break;
            }

        }
        public async Task ProcessReactionClickAction(string matchId, string connectionId, int timeTaken)
        {
            // must let match object handle the update as it handles the lock
            Match match = _activeMatchCache[Guid.Parse(matchId)];
            ReactionClickGameState gameState = match.UpdateReactionClickGame(connectionId, timeTaken);

            // other player has not yet clicked
            if (!gameState.Completed)
                return;

            IEnumerable<Player> players = match.Players.Values;

            // draw - select this request's player as the winner - maybe something special in future
            if (gameState.Winner is null)
            {
                await Task.WhenAll(players.Select(p => _matchHubContext.Clients.Group(matchId)
                    .UpdateReactionClickGame(new ReactionClickGameUpdateDto
                    {
                        Won = null
                    })
                ));

                return;
            }

            await Task.WhenAll(players.Select(p => _matchHubContext.Clients.GroupExcept(matchId, new string[] { match.GetOtherPlayer(p.ConnectionId).ConnectionId })
                .UpdateReactionClickGame(new ReactionClickGameUpdateDto
                {
                    Won = p.ConnectionId == gameState.Winner
                })
            ));
        }
    }
}
