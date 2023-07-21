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
        // connectionId of player that won the last game
        string _lastWinner;
        // for instances 
        readonly object _gameLock = new object();

        // games - must not be available to MatchManager
        ReactionClickGame _reactionClickGame;
        AimGame _aimGame;
        // having difficulty thinking about how to store different kinds of games nicely
        // they're too different to abstract via an interface but it feels bad hard coding each
        // of them into their own field

        public Guid Id { get; }
        public GameType CurrentGameType => _currentGameType;

        // getters for game info - avoid returning actual games so that manager cannot edit
        public int ReactionClickGameSetup => _reactionClickGame.TimeUntilScreen;
        public AimGameSetup AimGameSetup => new AimGameSetup 
        { 
            Turns = _aimGame.Turns, 
            TimeBetweenTurns = _aimGame.TimeBetweenTurns 
        };

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
            
            // every second game is an aim game
            if (_gameCounter % 2 == 0)
            {
                _currentGameType = GameType.Aim;
                // game becomes faster as the match goes on
                int timeBetweenTurns = 2000 - _gameCounter * 80;
                _aimGame = new AimGame(_lastWinner, GetOtherPlayer(_lastWinner).ConnectionId, timeBetweenTurns);
                return;
            }

            // otherwise randomly choose a game that isn't reaction click or aim
            _currentGameType = GameType.ReactionClick;
            _reactionClickGame = new ReactionClickGame();
        }
        
        public ReactionClickGameState UpdateReactionClickGameState(string connectionId, int timeTaken)
        {
            // lock so that both requests don't set both their times before either have checked whether other has clicked
            // this results in both returning the same thing
            lock (_gameLock)
            {
                // set a premature click to the max time
                _reactionClickGame.PlayerTimes[connectionId] = timeTaken <= 0 ? ReactionClickGame.MAX_TIME : timeTaken;

                Player otherPlayer = GetOtherPlayer(connectionId);
                bool otherPlayedClicked = _reactionClickGame.PlayerTimes.TryGetValue(otherPlayer.ConnectionId, out int otherPlayerTime);

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
                _lastWinner = winner;
                return new ReactionClickGameState { Winner = winner, Completed = completed };
            }
        }



        // receive aim action (could be a noAction)
        public AimGameState UpdateAimGameState(string connectionId, int? timeTaken)
        {
            lock (_gameLock)
            {
                int[] currentPlayerTimes =_aimGame.PlayerTimes[connectionId];

                // player did not act - set to max possible time
                if (timeTaken is null)
                {
                    currentPlayerTimes[_aimGame.TurnCount] = _aimGame.TimeBetweenTurns;
                }

                string otherPlayerId = GetOtherPlayer(connectionId).ConnectionId;
                int otherPlayerTime = _aimGame.PlayerTimes[otherPlayerId][_aimGame.TurnCount];

                // other player action not yet received
                if (otherPlayerTime == 0)
                {
                    // can't do anything yet
                    return null;
                }

                // in case of a tie, worse ping wins!
                string turnWinnerId = timeTaken <= otherPlayerTime ? connectionId : otherPlayerId;
                int winnerHealth = Players[turnWinnerId].Health;
                // loser has health deducted
                string turnLoserId = GetOtherPlayer(turnWinnerId).ConnectionId;
                int loserHealth = Players[turnLoserId].Health -= 10;

                // ensure match state is updated
                Dictionary<string, int> playerHealth = UpdateHealth(turnWinnerId, winnerHealth, turnLoserId, loserHealth);
                _aimGame.IncrementTurnCount();

                return new AimGameState
                {
                    Winner = turnWinnerId,
                    PlayerHealth = playerHealth
                };
            }
        }

        private Dictionary<string, int> UpdateHealth(string player1Id, int player1Hp, string player2Id, int player2Hp)
        {
            Players[player1Id].Health = player1Hp;
            Players[player2Id].Health = player2Hp;

            return new Dictionary<string, int> { { player1Id, player1Hp }, { player2Id, player2Hp } };
        }
    }
}
