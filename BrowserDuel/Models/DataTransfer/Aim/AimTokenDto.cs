namespace BrowserDuel.Models.DataTransfer
{
    public record AimTokenDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Attack { get; set; }
    }
}
