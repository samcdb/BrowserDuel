namespace BrowserDuel.Models.DataTransfer
{
    // action sent from player
    public record AimActionDto
    {
        public string MatchId { get; set; }
        public int? TimeTaken { get; set; }
    }
}
