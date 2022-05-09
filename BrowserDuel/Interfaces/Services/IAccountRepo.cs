using BrowserDuel.Models;

namespace BrowserDuel.Interfaces
{
    public interface IAccountRepo
    {
        Task<Account> GetAccount(Guid id);
    }
}
