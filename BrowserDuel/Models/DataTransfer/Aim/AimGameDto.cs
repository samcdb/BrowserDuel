using BrowserDuel.Models.Games;
using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Models.DataTransfer
{
    // sent to player for game set up
    public record AimGameDto
    {
        public List<AimTokenDto> Turns { get; set; }
        public int TimeBetweenTurns { get; set; }
    }
}
