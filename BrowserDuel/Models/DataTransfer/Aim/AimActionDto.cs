namespace BrowserDuel.Models.DataTransfer
{
    public record AimActionDto
    {
        public string MatchId { get; set; }
        public int? TimeTaken { get; set; }
        public int Index { get; set; }
    }
}
