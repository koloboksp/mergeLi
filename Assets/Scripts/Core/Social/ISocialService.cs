using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;

namespace Core.Social
{
    public interface ISocialService
    {
        bool IsAutoAuthenticationAvailable();
        Task<bool> AuthenticateAsync(CancellationToken cancellationToken);
        bool IsAuthenticated();

        Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken);
        Task<bool> ShowLeaderboardUIAsync(string id, GameProcessor gameProcessor, CancellationToken cancellationToken);
        Task<bool> UnlockAchievementAsync(string id, CancellationToken cancellationToken);
        Task<bool> SetScoreForLeaderBoard(string id, long value, CancellationToken cancellationToken);
    }
}