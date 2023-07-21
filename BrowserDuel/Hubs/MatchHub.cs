using Microsoft.AspNetCore.SignalR;
using BrowserDuel.Interfaces;
using BrowserDuel.Models;
using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Hubs
{
    public class MatchHub : Hub<IMatchClient>
    {
        private readonly IMatchMakingService _matchMakingService;
        private readonly IAccountRepo _accountRepo;
        private readonly IMatchManager _matchManager;

        public MatchHub(IMatchMakingService matchMakingService, IAccountRepo accountRepo, IMatchManager matchManager)
        {
            _matchMakingService = matchMakingService;
            _accountRepo = accountRepo;
            _matchManager = matchManager;
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // do other things here
            Console.WriteLine($"Connection: {Context.ConnectionId}");
            await Clients.Caller.Connected("success");
        }

        // add player to match queue
        public async Task AddToQueue(string accountId)
        {
            Account playerAccount = await _accountRepo.GetAccount(new Guid(accountId));
            _matchMakingService.AddToQueue(new Player(Context.ConnectionId, playerAccount));
        }

        public async Task PlayerReady(string matchId)
        {
            Console.WriteLine($"MatchHub.PlayerReady - Player Ready: {Context.ConnectionId} Thread: {Thread.CurrentThread.ManagedThreadId}");
            await _matchManager.SetPlayerReady(matchId, Context.ConnectionId);
        }

        // Reaction Click Game
        public async Task ReactionClickAction(ReactionClickActionDto playerAction)
        {
            string matchId = playerAction.MatchId;
            int timeTaken = playerAction.TimeTaken; ;
            string connectionId = Context.ConnectionId;

            Console.WriteLine($"Player click action - connectionId: {connectionId} timeTaken: {timeTaken}");

            await _matchManager.ProcessReactionClickAction(matchId, Context.ConnectionId, timeTaken);
        }

        // Aim Game
        // process a player's click
        public async Task AimAction(AimActionDto playerAction)
        {
            string matchId = playerAction.MatchId;
            int? timeTaken= playerAction.TimeTaken;
            string connectionId = Context.ConnectionId;

            Console.WriteLine($"Player aim action - connectionId: {connectionId} timeTaken: {timeTaken}");

            await _matchManager.ProcessAimAction(matchId, Context.ConnectionId, timeTaken);
        }
    }
}
