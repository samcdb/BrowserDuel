using BrowserDuel.Enums;
using BrowserDuel.Models.Games;

namespace BrowserDuel.Models
{
    // this is for maintaining state of match
    // this manages the game lock to avoid data race issues
    // when updating game state - only return what the MatchManager actually needs to know, winner/loser
    public class Match
    {
        GameType _currentGameType;
        int _gameCounter;
        // for instances 
        readonly object _gameLock = new object();

        // games - must not be available to MatchManager
        ReactionClickGame _reactionClickGame;

        // having difficulty thinking about how to store different kinds of games nicely
        // they're too different to abstract via an interface but it feels bad hard coding each
        // of them into their own field

        public Guid Id { get; }
        public GameType CurrentGameType => _currentGameType;
        // getters for game info - avoid returning actual games so that manager cannot edit
        // reaction click game
        public int ReactionClickGameSetup => _reactionClickGame.TimeUntilScreen;
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
        
        public ReactionClickGameState UpdateReactionClickGame(string connectionId, int timeTaken)
        {
            // lock so that both requests don't set both their times before either have checked whether other has clicked
            // this results in both returning the same thing - one should short circuit and return not completed
            lock (_gameLock)
            {
                Dictionary<string, int> playerTimes = _reactionClickGame.PlayerTimes;
                // set a premature click to the max time
                playerTimes[connectionId] = timeTaken <= 0 ? ReactionClickGame.MAX_TIME : timeTaken;

                Player otherPlayer = GetOtherPlayer(connectionId);
                bool otherPlayedClicked = playerTimes.TryGetValue(otherPlayer.ConnectionId, out int otherPlayerTime);

                bool completed = false;
                // other player has not clicked yet
                if (!otherPlayedClicked)
                    return new ReactionClickGameState { Completed = completed };

                completed = true;
                string winner = null;

                if (timeTaken < otherPlayerTime)
                    winner = connectionId;

                if (timeTaken > otherPlayerTime)
                    winner = otherPlayer.ConnectionId;

                // otherwise it's a draw

                return new ReactionClickGameState { Winner = winner, Completed = completed };
            }
        }

    }
}
