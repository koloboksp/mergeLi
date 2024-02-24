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
    }
}