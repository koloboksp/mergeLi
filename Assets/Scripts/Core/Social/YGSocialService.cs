using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using YG;

namespace Core.Social
{
    public class YGSocialService : ISocialService
    {
        public bool IsAutoAuthenticationAvailable()
        {
            return false;
        }

        public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken)
        {
            return false;
        }

        public bool IsAuthenticated()
        {
            return true;
        }

        public async Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken)
        {
            return false;
        }

        public async Task<bool> ShowLeaderboardUIAsync(string id, GameProcessor gameProcessor, CancellationToken cancellationToken)
        {
            var panelData = new UIYGLeaderboardPanelData();
            panelData.Id = id;
            panelData.GameProcessor = gameProcessor;
            var pushPopupScreenAsync = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIYGLeaderboardPanel>(
                panelData,
                cancellationToken);
            await pushPopupScreenAsync.ShowAsync(cancellationToken);
            
            return true;
        }

        public async Task<bool> UnlockAchievementAsync(string id, CancellationToken cancellationToken)
        {
            return false;
        }
        
        public async Task<bool> SetScoreForLeaderBoard(string id, long value, CancellationToken cancellationToken)
        {
            
            YG2.SetLeaderboard(id, (int)value);
            return true;
        }
    }
}