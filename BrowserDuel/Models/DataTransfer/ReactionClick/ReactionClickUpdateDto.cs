namespace BrowserDuel.Models.DataTransfer
{
    public record ReactionClickUpdateDto
    {
        public bool? Won { get; set; }// null if draw
    }
}
