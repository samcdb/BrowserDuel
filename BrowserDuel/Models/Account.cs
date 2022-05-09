namespace BrowserDuel.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public int Elo { get; set; }

        public void Deconstruct(out Guid id, out int elo)
        {
            id = Id;
            elo = Elo;
        }
    }
}
