namespace BrowserDuel.Models.Games
{
    public class AimGame
    {
        int _timeBetweenTurns;

        public Dictionary<string, int> PlayerTimes { get; set; }
        public IList<AimToken> Turns { get; }
        public int TimeBetweenTurns => _timeBetweenTurns;

        public AimGame(string playerOne, string playerTwo, int timeBetweenTurns)
        {
            _timeBetweenTurns = timeBetweenTurns;
            int numberOfTurns = new Random().Next(8, 12);

            if (numberOfTurns % 2 == 1)
                numberOfTurns++;

            Turns = new AimToken[numberOfTurns];

            for (int i = 0; i < numberOfTurns; i++)
            {
                Turns[i] = new AimToken(i % 2 == 0 ? playerOne : playerTwo);
            }
        }
    }
}
