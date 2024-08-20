
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;

namespace Core.Social
{
    public class DefaultSocialService : ISocialService
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
            return false;
        }

        public async Task<bool> ShowAchievementsUIAsync(CancellationToken cancellationToken)
        {
            return false;
        }

        public async Task<bool> ShowLeaderboardUIAsync(string id, CancellationToken cancellationToken)
        {
            return false;
        }

        public async Task<bool> UnlockAchievementAsync(string id, CancellationToken cancellationToken)
        {
            return false;
        }
        
        public async Task<bool> SetScoreForLeaderBoard(string id, long value, CancellationToken cancellationToken)
        {
            return false;
        }
    }
}