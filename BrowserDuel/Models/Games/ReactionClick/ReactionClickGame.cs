namespace BrowserDuel.Models.Games
{
    // this should only be visible to Match object
    public class ReactionClickGame
    {
        public const int MAX_TIME = 5000;
        // time until screen appears -> prompting players to click
        int _timeUntilScreen;
        public int TimeUntilScreen { get => _timeUntilScreen; }
        // store reaction times of both players
        public Dictionary<string, int> PlayerTimes { get; set; }

        public ReactionClickGame()
        {
            _timeUntilScreen = new Random().Next(3000, 6000);
            PlayerTimes = new Dictionary<string, int>();
        }
    }
}
