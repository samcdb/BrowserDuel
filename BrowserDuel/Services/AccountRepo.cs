using BrowserDuel.Interfaces;
using BrowserDuel.Models;

namespace BrowserDuel.Services
{
    public class AccountRepo : IAccountRepo
    {
        public async Task<Account> GetAccount(Guid id)
        {
            // TODO: link to database
            return new Account() { Id = id, Elo = 1500};
        }
    }
}
