
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;

namespace Core.Social
{
    public class Default : ISocialService
    {
        public bool IsAutoAuthenticationAvailable()
        {
            return true;
        }

        public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var timer = new SmallTimer();

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