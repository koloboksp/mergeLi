using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface ISocialService
    {
        bool IsAutoAuthenticationAvailable();
        Task<bool> AuthenticateAsync(CancellationToken cancellationToken);
        bool IsAuthenticated();

        Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken);
        Task<bool> ShowLeaderboardUIAsync(CancellationToken cancellationToken);
        Task<bool> UnlockAchievementAsync(string id, CancellationToken cancellationToken);
        Task<bool> SetScoreForLeaderBoard(string leaderBoard, long value, CancellationToken cancellationToken);
    }
}