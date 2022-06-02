namespace BrowserDuel.Models.DataTransfer
{
    public class AimTokenDto
    {
        // can't send tuple in dto
        public int X { get; set; }
        public int Y { get; set; }
        public bool Attack { get; set; }
    }
}
