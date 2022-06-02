using BrowserDuel.Models.Games;

namespace BrowserDuel.Models.Games
{
    public class AimGameSetup
    {
        public AimToken[] Turns { get; set; }
        public int TimeBetweenTurns { get; set; }
    }
}
