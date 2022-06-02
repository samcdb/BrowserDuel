using BrowserDuel.Models.DataTransfer;

namespace BrowserDuel.Interfaces
{
    public interface IMatchClient
    {
        Task Connected(string message);
        Task MatchFound(MatchFoundDto match);
        Task StartReactionClickGame(ReactionClickGameDto reactionClickGame);
        Task UpdateReactionClickGame(ReactionClickGameUpdateDto gameState);
        Task StartAimGame(AimGameDto aimGame);
    }
}
