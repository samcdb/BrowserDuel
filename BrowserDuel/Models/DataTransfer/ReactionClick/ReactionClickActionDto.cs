namespace BrowserDuel.Models.DataTransfer
{
    // player action
    public record ReactionClickActionDto
    {
        public string MatchId { get; set; }
        public int TimeTaken { get; set; }
    }
}
