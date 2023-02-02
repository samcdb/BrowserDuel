using BrowserDuel.Models.Games;
using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Models.DataTransfer
{
    public record AimGameDto
    {
        public List<AimTokenDto> Turns { get; set; }
        public int TimeBetweenTurns { get; set; }
    }
}
