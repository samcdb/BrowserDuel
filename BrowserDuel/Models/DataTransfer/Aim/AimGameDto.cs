using BrowserDuel.Models.Games;
using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Models.DataTransfer
{
    public class AimGameDto
    {
        public IList<AimTokenDto> Turns { get; set; }
        public int TimeBetweenTurns { get; set; }
    }
}
