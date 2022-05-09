namespace BrowserDuel.Models
{
    public class Player
    {
        private int _health = 100;
        public string ConnectionId { get; }
        public Guid AccountId { get; }
        public int Elo { get; }
        public int Health { get => _health; set => _health = value; }
        // won last mini game - decides if first to attack in next aim game
        public bool WonLastGame { get; set; }

        public Player(string connectionId, Account playerAccount)
        {
            ConnectionId = connectionId;
            (AccountId, Elo) = playerAccount;
        }
    }
}
