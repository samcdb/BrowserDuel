namespace BrowserDuel.Models.Games
{
    public readonly struct AimToken
    {
        public readonly int X;
        public readonly int Y;
        public readonly string AttackerId;

        public AimToken(string attackerId)
        {
            AttackerId = attackerId;

            var randomiser = new Random();
            X = randomiser.Next(0, 100);
            Y = randomiser.Next(0, 100); ;
        }
    }
}
