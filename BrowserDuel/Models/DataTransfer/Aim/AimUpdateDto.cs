namespace BrowserDuel.Models.DataTransfer
{
    public record AimUpdateDto
    {
        public bool Won { get; set; }
        public bool GameFinished { get; set; }
        public Dictionary<string, int> PlayerHealth { get; set; }
    }
}
