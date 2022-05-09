using Microsoft.AspNetCore.SignalR;
using BrowserDuel.Interfaces;
using BrowserDuel.Models;

namespace BrowserDuel.Hubs
{
    public class MatchHub : Hub<IMatchClient>
    {
        private readonly IMatchMakingService _matchMakingService;
        private readonly IAccountRepo _accountRepo;

        public MatchHub(IMatchMakingService matchMakingService, IAccountRepo accountRepo)
        {
            _matchMakingService = matchMakingService;
            _accountRepo = accountRepo;
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // do other things here
            Console.WriteLine($"Connection: {Context.ConnectionId}");
        }

        // add player to match queue
        public async Task AddToQueue(string accountId)
        {
            Account playerAccount = await _accountRepo.GetAccount(new Guid(accountId));
            _matchMakingService.AddToQueue(new Player(Context.ConnectionId, playerAccount));
        }
    }
}
