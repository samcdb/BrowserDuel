using BrowserDuel.Enums;
using BrowserDuel.Models.Games;

namespace BrowserDuel.Models
{
    // this is purely for maintaining state of match
    public class Match
    {
        GameType _currentGameType;
        int _gameCounter;
        // for instances 
        readonly object _gameLock = new object();

        // games
        ReactionClickGame _reactionClickGame;

        // having difficulty thinking about how to store different kinds of games nicely
        // they're too different to abstract via an interface but it feels bad hard coding each
        // of them into their own field

        public Guid Id { get; }
        // a match is made up of games
        public GameType CurrentGameType { get => _currentGameType; }
        public ReactionClickGame ReactionClickGame { get => _reactionClickGame; }
        public int GameCounter { get => _gameCounter; }
        // quick look up n(1) < n(2)
        public Dictionary<string, Player> Players { get; }

        public Match(Player[] players)
        {
            Id = Guid.NewGuid();
            Players = players.ToDictionary(p => p.ConnectionId);
            _currentGameType = GameType.ReactionClick;
            _gameCounter = 0;
        }

        // get player that does not have this connection id
        // used when sending messages specifically tailored to players
        public Player GetOtherPlayer(string connectionId)
        {
            return Players.Values.Where(p => p.ConnectionId != connectionId).FirstOrDefault();
        }

        //  set player to be ready for next game - if other player is ready as well then return true to show 
        // both players are ready for next game
        public bool ReadyForNextGame(string connectionId)
        {
            // if no lock and both requests come in at same time ->
            // both players are set to ready before checking if other player is ready ->
            // both requests incorrectly call StartNextGame
            lock (_gameLock)
            {
                Players[connectionId].ReadyForNextGame = true;
                return GetOtherPlayer(connectionId).ReadyForNextGame;
            }
        }

        public void NextGame()
        {
            _gameCounter++;
            
            foreach (Player p in Players.Values)
            {
                p.ReadyForNextGame = false;
            }

            // if match is starting - reaction game
            if (_gameCounter == 1)
            {
                _currentGameType = GameType.ReactionClick;
                _reactionClickGame = new ReactionClickGame();
                return;
            }
            
            if (_gameCounter % 2 == 0)
            {
                _currentGameType = GameType.Aim;
            }

            
        }

    }
}
