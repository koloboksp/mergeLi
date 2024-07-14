using Core;
using Core.Gameplay;
using UnityEngine;

namespace Achievements
{
    public class Achievement : MonoBehaviour
    {
        [SerializeField] private string _id;
        
        private GameProcessor _gameProcessor;

        public string Id => _id;
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