#if UNITY_IOS
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

namespace Core.Social
{
    public class AppleGameCenter : ISocialService
    {
        public bool IsAutoAuthenticationAvailable()
        {
            return false;
        }

        public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var authCompletionSource = new TaskCompletionSource<bool>();
            var exitCancellationTokenRegistration = cancellationToken.Register(() => { authCompletionSource.TrySetCanceled(cancellationToken); });
            try
            {
                UnityEngine.Social.localUser.Authenticate(success =>
                {
                    Debug.Log(success 
                        ? $"<color=#99ff99>Authenticate success.</color>" 
                        : $"<color=#99ff99>Authenticate failed.</color>");

                    authCompletionSource.TrySetResult(success);
                });

                return await authCompletionSource.Task;
            }
            finally
            {
                exitCancellationTokenRegistration.Dispose();
            } 
        }

        public bool IsAuthenticated()
        {
            return UnityEngine.Social.localUser.authenticated;
        }

        public async Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken)
        {
            if (!UnityEngine.Social.localUser.authenticated)
            {
                var authResult = await AuthenticateAsync(cancellationToken);
                if (!authResult)
                {
                    return false;
                }
            }
            
            UnityEngine.Social.ShowAchievementsUI();
            return true;
        }

        public async Task<bool> ShowLeaderboardUIAsync(string id, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken));
            
            if (!UnityEngine.Social.localUser.authenticated)
            {
                var authResult = await AuthenticateAsync(cancellationToken);
                if (!authResult)
                {
                    return false;
                }
            }
            
            GameCenterPlatform.ShowLeaderboardUI(null, TimeScope.AllTime);
            
            try
            {
                return await completionSource.Task;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
            }
        }

        public async Task<bool> UnlockAchievementAsync(string id, CancellationToken cancellationToken)
        {
            Debug.Log($"<color=#99ff99>Try to unlock achievement '{id}'.</color>");

            var completionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken));

            if (!UnityEngine.Social.localUser.authenticated)
            {
                var authResult = await AuthenticateAsync(cancellationToken);
                if (!authResult)
                {
                    return false;
                }
            }
            
            UnityEngine.Social.ReportProgress(id, 100.0f, success =>
            {
                Debug.Log(success 
                    ? $"<color=#99ff99>Unlock achievement '{id}' success.</color>"
                    : $"<color=#99ff99>Unlock achievement '{id}' failed.</color>");

                completionSource.TrySetResult(success);
            });
            
            try
            {
                return await completionSource.Task;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
            }
        }
        
        public async Task<bool> SetScoreForLeaderBoard(string id, long value, CancellationToken cancellationToken)
        {
            var reportScore = new TaskCompletionSource<bool>();
            var cancellationExitTokenRegistration = cancellationToken.Register(() => reportScore.TrySetCanceled(cancellationToken));
            try
            {
                if (!UnityEngine.Social.localUser.authenticated)
                {
                    var authResult = await AuthenticateAsync(cancellationToken);
                    if (!authResult)
                    {
                        return false;
                    }
                }
                
                UnityEngine.Social.ReportScore(value, id, (success) =>
                {
                    Debug.Log(success
                        ? $"<color=#99ff99>Set score for leaderboard '{id}' success.</color>" 
                        : $"<color=#99ff99>Set score for leaderboard '{id}' failed.</color>");

                    reportScore.TrySetResult(success);
                });
                return await reportScore.Task;
            }
            finally
            {
                cancellationExitTokenRegistration.Dispose();
            }
        }
    }
}
#endif