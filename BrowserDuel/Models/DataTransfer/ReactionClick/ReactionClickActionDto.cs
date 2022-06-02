namespace BrowserDuel.Models.DataTransfer
{
    // player action
    public class ReactionClickActionDto
    {
        public string MatchId { get; set; }
        public int TimeTaken { get; set; }
        
        public void Deconstruct(out string matchId, out int timeTaken)
        {
            matchId = MatchId;
            timeTaken = TimeTaken;
        }
    }
}
