using System;
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

        public async Task Authenticate()
        {
            try
            {
                Debug.Log($"<color=#99ff99>Initialize {nameof(PlayGamesPlatform)}.</color>");
            
                PlayGamesPlatform.DebugLogEnabled = true;

                var timer = new SmallTimer();
            
                PlayGamesPlatform.Instance.Authenticate(status =>
                {
                    Debug.Log(status == SignInStatus.Success
                        ? $"<color=#00CCFF>Play Games sign in. UserName: {PlayGamesPlatform.Instance.localUser.userName}.</color>"
                        : $"<color=#00CCFF>Failed to sign into Play Games Services: {status}.</color>");
                });

                Debug.Log($"<color=#99ff99>Time authenticate {nameof(PlayGamesPlatform)}: {timer.Update()}.</color>");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public bool IsAuthenticated()
        {
#if UNITY_ANDROID
            return PlayGamesPlatform.Instance.IsAuthenticated();
#else
            throw new Exception("This platform is not supported.");
#endif
        }
    }
}