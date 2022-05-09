using BrowserDuel.Models;

namespace BrowserDuel.Events
{
    public class MatchFoundEventArgs : EventArgs
    {
        public Match NewMatch { get; set; }

        public MatchFoundEventArgs(Match newMatch)
        {
            NewMatch = newMatch;
        }
    }
}
