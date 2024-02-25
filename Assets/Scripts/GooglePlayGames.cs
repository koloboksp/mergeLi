using System;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Core
{
    public class GooglePlayGames : ISocialService
    {
        public bool IsAutoAuthenticationAvailable()
        {
            return true;
        }

        public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var timer = new SmallTimer();
            
            var cancellationTokenCompletion = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => cancellationTokenCompletion.SetResult(true));
        
            var signInCompletion = new TaskCompletionSource<SignInStatus>();
            
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                signInCompletion.SetResult(status);
            });
            
            await Task.WhenAny(signInCompletion.Task, cancellationTokenCompletion.Task);

            if (signInCompletion.Task.IsCompleted)
            {
                Debug.Log(signInCompletion.Task.Result == SignInStatus.Success
                    ? $"<color=#00CCFF>Play Games sign in. UserName: {PlayGamesPlatform.Instance.localUser.userName}.</color>"
                    : $"<color=#00CCFF>Failed to sign into Play Games Services: {signInCompletion.Task.Result}.</color>");
                
                Debug.Log($"<color=#99ff99>Time authenticate {nameof(PlayGamesPlatform)}: {timer.Update()}.</color>");
                
                return signInCompletion.Task.Result == SignInStatus.Success;
            }

            throw new OperationCanceledException();
        }
        
        public bool IsAuthenticated()
        {
#if UNITY_ANDROID
            return PlayGamesPlatform.Instance.IsAuthenticated();
#else
            throw new Exception("This platform is not supported.");
#endif
        }

        public async Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken)
        {
            var uiShowing = true;
            PlayGamesPlatform.Instance.ShowAchievementsUI((status) =>
            {
                uiShowing = false;
            });

            while (uiShowing)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Yield();
            }

            return true;
        }
        
        public async Task<bool> ShowLeaderboardUIAsync(CancellationToken cancellationToken)
        {
            var uiShowing = true;
            PlayGamesPlatform.Instance.ShowLeaderboardUI(null, (status) =>
            {
                uiShowing = false;
            });

            while (uiShowing)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            return true;
        }
    }
}