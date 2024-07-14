using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Core.Social
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

            PlayGamesPlatform.Instance.Authenticate(status => { signInCompletion.SetResult(status); });

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
            var completionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken));

            PlayGamesPlatform.Instance.ShowAchievementsUI((status) => { completionSource.TrySetResult(true); });

            try
            {
                return await completionSource.Task;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();
            }
        }

        public async Task<bool> ShowLeaderboardUIAsync(CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken));

            PlayGamesPlatform.Instance.ShowLeaderboardUI(null, (status) => { completionSource.TrySetResult(true); });

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

            PlayGamesPlatform.Instance.UnlockAchievement(null, (status) =>
            {
                if (status)
                    Debug.Log($"<color=#99ff99>Unlock achievement '{id}' success.</color>");
                else
                    Debug.Log($"<color=#99ff99>Unlock achievement '{id}' failed.</color>");

                completionSource.TrySetResult(status);
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
        
        public async Task<bool> SetScoreForLeaderBoard(string leaderBoard, long value, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken));

            PlayGamesPlatform.Instance.ReportScore(value, leaderBoard, status =>
            {
                if (status)
                    Debug.Log($"Score: {value} was success set for the leaderboard.");
                else
                    Debug.Log(new Exception("Score is not set for the leaderboard."));

                completionSource.SetResult(status);
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
    }
}