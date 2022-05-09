using BrowserDuel.Enums;
using BrowserDuel.Models;

namespace BrowserDuel.Models
{
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

        public Match(Player[] players)
        {
            Id = new Guid();
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
