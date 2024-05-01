using Core;
using UnityEngine;

namespace Achievements
{
    public class Achievement : MonoBehaviour
    {
        private GameProcessor _gameProcessor;

        public string Id => gameObject.name;
        public GameProcessor GameProcessor => _gameProcessor;

        protected virtual void InnerCheck()
        {
        }

        public virtual void SetData(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;
        }

        protected void Unlock()
        {
            _ = ApplicationController.Instance.ISocialService.UnlockAchievementAsync(Id, Application.exitCancellationToken);
        }
    }
}