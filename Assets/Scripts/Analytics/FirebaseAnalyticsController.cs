using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics;
using Core.Gameplay;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace Core
{
    public class FirebaseAnalyticsController : IAnalyticsController
    {
        private const string VERSION = "VERSION";
        
        private const string TUTORIAL_STEP_NAME = "TUTORIAL_STEP_NAME";
        private const string STEP = "STEP";
        private const string BUFF = "BUFF";
        private const string ADS = "ADS";
        private const string CASTLE_ID = "CASTLE_ID";
        private const string REST_CELLS = "REST_CELLS";
        private const string GIFT = "GIFT";
        private const string REWARD_AMOUNT = "REWARD_AMOUNT";
        private const string PRODUCT = "PRODUCT";

        private const string TUTORIAL_STEP_COMPLETE = "TUTORIAL_STEP_COMPLETE";
        private const string STEP_INTERVAL_COMPLETE = "STEP_INTERVAL_COMPLETE";
        private const string BUFF_USED = "BUFF_USED";
        private const string ADS_VIEWED = "ADS_VIEWED";
        private const string LOST = "LOST";
        private const string RESTART = "RESTART";
        private const string GIFT_COLLECTED = "GIFT_COLLECTED";
        private const string MARKET_BOUGHT = "MARKET_BOUGHT";
        private const string CASTLE_COMPLETED = "CASTLE_COMPLETED";

        private FirebaseApp _firebase;
        private Atom.Version _version;

        public async Task InitializeAsync(Atom.Version version)
        {
            _version = version;

            var fixDependencies = FirebaseApp.CheckAndFixDependenciesAsync();
            await fixDependencies;

            if (fixDependencies.Result == DependencyStatus.Available)
            {
                _firebase = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Firebase not initialized: '{fixDependencies.Result}'");
            }
        }

        public void TutorialStepCompleted(string stepName)
        {
            if (_firebase == null)
                return;

            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(TUTORIAL_STEP_NAME, stepName)
                };
                FirebaseAnalytics.LogEvent(
                    TUTORIAL_STEP_COMPLETE,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {TUTORIAL_STEP_COMPLETE} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void StepIntervalCompleted(int step, string castleId, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new LongParameterWrapper(STEP, step),
                    new StringParameterWrapper(CASTLE_ID, castleId)
                };
                parameters.AddRange(stepsTakenIntoInfo
                    .Select(i => new LongParameterWrapper(i.Tag.ToString(), i.Count)));

                FirebaseAnalytics.LogEvent(
                    STEP_INTERVAL_COMPLETE,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {STEP_INTERVAL_COMPLETE} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void BuffUsed(StepTag buffTag, string castleId, int step, int restFieldEmptyCellsCount)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(BUFF, buffTag.ToString()),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };

                FirebaseAnalytics.LogEvent(
                    BUFF_USED,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {BUFF_USED} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void AdsViewed(string adsName, int rewardAmount, string castleId, int step)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(ADS, adsName),
                    new LongParameterWrapper(REWARD_AMOUNT, rewardAmount),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };

                FirebaseAnalytics.LogEvent(
                    ADS_VIEWED,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {ADS_VIEWED} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnLost(string castleId, int step, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };
                parameters.AddRange(stepsTakenIntoInfo
                    .Select(i => new LongParameterWrapper(i.Tag.ToString(), i.Count)));

                FirebaseAnalytics.LogEvent(
                    LOST,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {LOST} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnRestart(string castleId, int step, int restFieldEmptyCellsCount, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                    new LongParameterWrapper(REST_CELLS, restFieldEmptyCellsCount)
                };
                parameters.AddRange(stepsTakenIntoInfo
                    .Select(i => new LongParameterWrapper(i.Tag.ToString(), i.Count)));

                FirebaseAnalytics.LogEvent(
                    RESTART,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {RESTART} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnGiftCollected(string id, int rewardAmount, string castleId, int step)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(GIFT, id),
                    new LongParameterWrapper(REWARD_AMOUNT, rewardAmount),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };
                
                FirebaseAnalytics.LogEvent(
                    GIFT_COLLECTED,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {GIFT_COLLECTED} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnMarketBought(string id, int rewardAmount, string castleId, int step, int restFieldEmptyCellsCount)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(PRODUCT, id),
                    new LongParameterWrapper(REWARD_AMOUNT, rewardAmount),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };

                FirebaseAnalytics.LogEvent(
                    MARKET_BOUGHT,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {MARKET_BOUGHT} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnCastleComplete(string castleId, int step)
        {
            try
            {
                var parameters = new List<ParameterWrapper>
                {
                    new StringParameterWrapper(VERSION, _version.ToString()),
                    new StringParameterWrapper(CASTLE_ID, castleId),
                    new LongParameterWrapper(STEP, step),
                };

                FirebaseAnalytics.LogEvent(
                    CASTLE_COMPLETED,
                    parameters
                        .ConvertAll(i => i.ToParameter())
                        .ToArray());

                Debug.Log($"<color=Green>Analytics:</color> {CASTLE_COMPLETED} " +
                          $"{parameters.ConvertAll(i => i.ToString()).Aggregate((c, n) => $"{c} {n}")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public abstract class ParameterWrapper
        {
            private readonly string _name;

            protected string Name => _name;
            
            protected ParameterWrapper(string name)
            {
                _name = name;
            }

            public abstract Parameter ToParameter();
        }

        private class StringParameterWrapper : ParameterWrapper
        {
            private readonly string _value;
            
            public StringParameterWrapper(string name, string value) : base(name)
            {
                _value = value;
            }

            public override string ToString()
            {
                return $"{Name}:{_value}";
            }

            public override Parameter ToParameter()
            {
                return new Parameter(Name, _value);
            }
        }

        private class LongParameterWrapper : ParameterWrapper
        {
            private readonly long _value;
            
            public LongParameterWrapper(string name, long value) : base(name)
            {
                _value = value;
            }
            public override string ToString()
            {
                return $"{Name}:{_value}";
            }

            public override Parameter ToParameter()
            {
                return new Parameter(Name, _value);
            }
        }
    }
}