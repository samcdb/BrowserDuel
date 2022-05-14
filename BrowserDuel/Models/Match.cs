using BrowserDuel.Enums;
using BrowserDuel.Models;

namespace BrowserDuel.Models
{
    // this is purely for maintaining state of match
    public class Match
    {
        GameType _currentGameType;
        int _gameCounter;

        public Guid Id { get; }
        // a match is made up of games
        public GameType CurrentGameType { get => _currentGameType; }
        public int GameCounter { get => _gameCounter; }
        // quick look up n(1) < n(2)
        public Dictionary<string, Player> Players { get; }

        // get player that does not have this connection id
        // used when sending messages specifically tailored to players
        public Player GetOtherPlayer(string connectionId)
        {
            return Players.Values.Where(p => p.ConnectionId != connectionId).FirstOrDefault();
        }

        public Match(Player[] players)
        {
            Id = Guid.NewGuid();
            Players = players.ToDictionary(p => p.ConnectionId);
            _currentGameType = GameType.ReactionScreen;
            _gameCounter = 1;
        }

        // transition to next game
        public void NextGame()
        {
            //_currentGameType = _gameCounter % 2 == 1
        }

    }
}
