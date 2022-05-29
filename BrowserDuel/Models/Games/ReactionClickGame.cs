namespace BrowserDuel.Models.Games
{
    public class ReactionClickGame
    {
        // time until screen appears -> prompting players to click
        int _timeUntilScreen;
        public int TimeUntilScreen { get => _timeUntilScreen; }
        // store reaction times of both players
        public Dictionary<string, int> PlayerTimes { get; set; }

        public ReactionClickGame()
        {
            _timeUntilScreen = new Random().Next(2000, 5000);
            PlayerTimes = new Dictionary<string, int>();
        }
    }
}
