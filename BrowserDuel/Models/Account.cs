namespace BrowserDuel.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public int Elo { get; set; }
        public string Name { get; set; }

        public void Deconstruct(out Guid id, out int elo, out string name)
        {
            id = Id;
            elo = Elo;
            name = Name;
        }
    }
}
