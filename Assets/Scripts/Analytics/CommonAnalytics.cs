using System.Collections.Generic;
using Core;
using Core.Steps;
using UnityEngine;

namespace Achievements
{
    public class CommonAnalytics : MonoBehaviour
    {
        [SerializeField] private int _eventInterval = 25;
        [SerializeField] private StepTag[] _stepsTakenInto;

        [SerializeField] private List<StepTag> _buffsTakenInto;

        private GameProcessor _gameProcessor;
        private readonly List<(StepTag stepTag, int count)> _stepsTakenIntoInfo = new();
        private int _step;
        
        public virtual void SetData(GameProcessor gameProcessor)
        {
            ResetSession();

            _gameProcessor = gameProcessor;
            _gameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
            _gameProcessor.OnRestart += GameProcessor_OnRestart;
            _gameProcessor.OnLose += GameProcessor_OnLose;
            _gameProcessor.AdsViewer.OnShowAds += AdsViewer_OnShowAds;
            _gameProcessor.GiftsMarket.OnCollect += GiftsMarket_OnCollect;
            _gameProcessor.Market.OnBought += Market_OnBought;
            _gameProcessor.CastleSelector.OnCastleCompleted += CastleSelector_OnCastleCompleted;
        }

        private void GameProcessor_OnRestart()
        {
            ApplicationController.Instance.AnalyticsController.OnRestart(
                _gameProcessor.CastleSelector.ActiveCastle.Id,
                _step,
                _gameProcessor.GetField().CalculateEmptySpacesCount(),
                _stepsTakenIntoInfo);

            ResetSession();
        }

        private void GameProcessor_OnLose()
        {
            ApplicationController.Instance.AnalyticsController.OnLost(
                _gameProcessor.CastleSelector.ActiveCastle.Id,
                _step,
                _stepsTakenIntoInfo);

            ResetSession();
        }
        
        private void ResetSession()
        {
            _stepsTakenIntoInfo.Clear();
            foreach (var stepTag in _stepsTakenInto)
                _stepsTakenIntoInfo.Add((stepTag, 0));

            _step = 0;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType executionType)
        {
            var stepTag = step.Tag;

            var foundI = _stepsTakenIntoInfo.FindIndex(i => i.stepTag == stepTag);
            if (foundI >= 0)
            {
                var stepInfo = _stepsTakenIntoInfo[foundI];
                stepInfo.count++;
                _stepsTakenIntoInfo[foundI] = stepInfo;
                _step++;
            }

            if (_buffsTakenInto.Contains(step.Tag))
            {
                ApplicationController.Instance.AnalyticsController.BuffUsed(
                    step.Tag,
                    _gameProcessor.CastleSelector.ActiveCastle.Id,
                    _step,
                    _gameProcessor.GetField().CalculateEmptySpacesCount());
            }

            if (foundI >= 0 && _step % _eventInterval == 0)
            {
                ApplicationController.Instance.AnalyticsController.StepIntervalCompleted(
                    _step,
                    _gameProcessor.CastleSelector.ActiveCastle.Id,
                    _stepsTakenIntoInfo);
            }
        }

        private void AdsViewer_OnShowAds(bool success, string adsName, int rewardAmount)
        {
            if (success)
            {
                ApplicationController.Instance.AnalyticsController.AdsViewed(
                    adsName,
                    rewardAmount,
                    _gameProcessor.CastleSelector.ActiveCastle.Id,
                    _step);
            }
        }

        private void GiftsMarket_OnCollect(bool success, string id, int rewardAmount)
        {
            if (success)
            {
                ApplicationController.Instance.AnalyticsController.OnGiftCollected(
                    id,
                    rewardAmount,
                    _gameProcessor.CastleSelector.ActiveCastle.Id,
                    _step);
            }
        }

        private void Market_OnBought(bool success, string id, int rewardAmount)
        {
            if (success)
            {
                ApplicationController.Instance.AnalyticsController.OnMarketBought(
                    id,
                    rewardAmount,
                    _gameProcessor.CastleSelector.ActiveCastle.Id,
                    _step,
                    _gameProcessor.GetField().CalculateEmptySpacesCount());
            }
        }
        
        private void CastleSelector_OnCastleCompleted()
        {
            ApplicationController.Instance.AnalyticsController.OnCastleComplete(
                _gameProcessor.CastleSelector.ActiveCastle.Id,
                _step);
        }
    }
}