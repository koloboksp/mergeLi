﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analytics;
using Atom;
using Core.Goals;
using Core.Steps;
using Core.Utils;
using Save;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Gameplay
{
    public class SessionProcessor : MonoBehaviour,
        ISessionStatisticsHolder,
        ISessionProgressHolder
    {
        public const string BEST_SESSION_SCORE_LEADERBOARD = "CgkIlqCktsMPEAIQEg";
    
        public static readonly List<StepTag> NonSaveTags = new()
        {
            { StepTag.Select },
            { StepTag.Deselect },
            { StepTag.ChangeSelected },
            { StepTag.NoPath}
        };
    
        public event Action OnLose;
        public event Action OnRestart;
        public event Action<int> OnScoreChanged;
        public event Action<bool> OnLowEmptySpaceChanged;
        public event Action<bool, bool> OnFreeSpaceIsOverChanged; 
        public event Action<Castle> OnCastleCompleted;
    
        [SerializeField] private bool _enableTutorial;
        [SerializeField] private bool _forceTutorial;
    
        [SerializeField] private AudioClip _failClip;


        private CancellationTokenSource _restartTokenSource;
        private CancellationTokenSource _loseTokenSource;
        private readonly List<CompletedCastleDesc> _completedCastles = new();
        private int _collapsedLinesCount;
        private int _mergedBallsCount;

        private GameProcessor _gameProcessor;
        private DependencyHolder<UIPanelController> _panelController;
        private DependencyHolder<SaveController> _saveController;
        
        private bool _userStepFinished = false;
        private bool _notAllBallsGenerated = false;
    
        public void SetData(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;
        }

        public async Task ProcessGameAsyncSafe(CancellationToken exitToken)
        {
            try
            {
                var activeSkinName = ApplicationController.Instance.SaveController.SaveSettings.ActiveSkin;
                var userInactiveHatsFilter = ApplicationController.Instance.SaveController.SaveSettings.UserActiveHatsFilter;
                _gameProcessor.Scene.SetData(activeSkinName, userInactiveHatsFilter);
            
                UIGameScreen gameScreen = null;
                UIStartPanel startPanel = null;
            
                if (_enableTutorial 
                    && _gameProcessor.TutorialController.CanStartTutorial(_forceTutorial && Application.isEditor))
                {
                    _gameProcessor.MusicPlayer.PlayNext();

                    await _gameProcessor.TutorialController.TryStartTutorial(_forceTutorial, exitToken);
                    gameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
                }
                else
                {
                    _gameProcessor.MusicPlayer.PlayNext();
                
                    startPanel = await _panelController.Value.PushScreenAsync<UIStartPanel>(
                        new UIStartPanelData()
                        {
                            GameProcessor = _gameProcessor,
                            Instant = false,
                        },
                        exitToken);
                
                    ApplicationController.UnloadLogoScene();
                    await startPanel.ShowAsync(exitToken);
                
                    PrepareSession();
                
                    gameScreen = await _panelController.Value.PushPopupScreenAsync<UIGameScreen>(
                        new UIGameScreenData()
                        {
                            Layer = "gameScreenLayer",
                            GameProcessor = _gameProcessor
                        },
                        exitToken);
                }
            
                while (true)
                {
                    exitToken.ThrowIfCancellationRequested();

                    _restartTokenSource = new CancellationTokenSource();
                    _loseTokenSource = new CancellationTokenSource();

                    var restartToken = _restartTokenSource.Token;
                    var loseToken = _loseTokenSource.Token;
               
                    try
                    {
                        await ProcessSessionAsync(gameScreen, restartToken, loseToken, exitToken);
                    }
                    catch (OperationCanceledException exception)
                    {
                        if (exception.CancellationToken == restartToken)
                        {
                            try
                            {
                                OnRestart?.Invoke();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        
                            _saveController.Value.SaveProgress.SetBestSessionResults(
                                GetScore(),
                                GetCollapseLinesCount(),
                                GetMergedBallsCount());
                            _saveController.Value.SaveLastSessionProgress.Clear();
                            _completedCastles.Clear();
                            _collapsedLinesCount = 0;
                            _mergedBallsCount = 0;
                            
                            foreach (var buff in _gameProcessor.Buffs)
                                buff.SetRestCooldown(0);
                        
                            _gameProcessor.MusicPlayer.PlayNext();
                        
                            PrepareSession();
                        }
                        else if (exception.CancellationToken == loseToken)
                        {
                            try
                            {
                                OnLose?.Invoke();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                            gameScreen.LockInput(true);
                            _gameProcessor.Field.View.LockInput(true);
                        
                            // Turn on win music
                            _gameProcessor.MusicPlayer.Stop();
                            _gameProcessor.SoundsPlayer.PlayExclusive(_failClip);
                        
                            await AsyncExtensions.WaitForSecondsAsync(1.0f, exitToken);

                            var balls = _gameProcessor.Scene.Field.GetAll<Ball>()
                                .ToList();

                            var ballsToSelect = new List<Ball>();
                            for (var i = 0; i < 10; i++)
                            {
                                var rIndex = Random.Range(0, balls.Count);
                                ballsToSelect.Add(balls[rIndex]);
                                balls.RemoveAt(rIndex);
                            }

                            ballsToSelect = ballsToSelect.OrderByDescending(i => i.GridPosition.y)
                                .ToList();
                            foreach (var ball in ballsToSelect)
                                ball.Select(true);

                            // Wait while the user watches for happy blobs
                            await AsyncExtensions.WaitForSecondsAsync(2.5f, exitToken);
                        
                            var failPanel = await _panelController.Value.PushPopupScreenAsync<UIGameFailPanel>(
                                new UIGameFailPanelData() { GameProcessor = _gameProcessor }, 
                                exitToken);

                            await failPanel.ShowAsync(exitToken);
                        
                       
                            // Turn on environment music
                            _gameProcessor.MusicPlayer.PlayNext();
                            _gameProcessor.SoundsPlayer.StopPlayExclusive();
                        
                            _saveController.Value.SaveProgress.SetBestSessionResults(
                                GetScore(),
                                GetCollapseLinesCount(),
                                GetMergedBallsCount());
                            _saveController.Value.SaveLastSessionProgress.Clear();
                            _completedCastles.Clear();
                            _collapsedLinesCount = 0;
                            _mergedBallsCount = 0;
                            
                            foreach (var buff in _gameProcessor.Buffs)
                                buff.SetRestCooldown(0);
                        
                            _ = ApplicationController.Instance.ISocialService.SetScoreForLeaderBoard(
                                BEST_SESSION_SCORE_LEADERBOARD, 
                                _saveController.Value.SaveProgress.BestSessionScore, 
                                Application.exitCancellationToken);

                            _gameProcessor.MusicPlayer.PlayNext();
                        
                            startPanel = await _panelController.Value.PushScreenAsync<UIStartPanel>(
                                new UIStartPanelData()
                                {
                                    GameProcessor = _gameProcessor,
                                    Instant = true,
                                },
                                exitToken);
                            await startPanel.ShowAsync(exitToken);
                        
                            PrepareSession();
                            gameScreen = await _panelController.Value.PushPopupScreenAsync<UIGameScreen>(
                                new UIGameScreenData()
                                {
                                    Layer = "gameScreenLayer",
                                    GameProcessor = _gameProcessor
                                },
                                exitToken);
                        
                            gameScreen.LockInput(false);
                            _gameProcessor.Field.View.LockInput(false);
                        }
                        else if (exception.CancellationToken == exitToken)
                        {
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        await Task.Yield();
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
        }
    
        private void PrepareSession()
        {
            if (HasPreviousSessionGame)
            {
                var lastSessionProgress = ApplicationController.Instance.SaveController.SaveLastSessionProgress;

                _completedCastles.AddRange(lastSessionProgress.CompletedCastles.Select(i => new CompletedCastleDesc(i.Id, i.Points)));
                _gameProcessor.CastleSelector.SelectActiveCastle(lastSessionProgress.ActiveCastle.Id);
                _gameProcessor.CastleSelector.ActiveCastle.SetPoints(lastSessionProgress.ActiveCastle.Points, true);
                _collapsedLinesCount = lastSessionProgress.CollapseLinesCount;
                _mergedBallsCount = lastSessionProgress.MergedBallsCount;

                var ballsProgressData = lastSessionProgress.Field.Balls.Select(i => new BallDesc(i.GridPosition, i.Points, i.HatHame));
                _gameProcessor.Field.AddBalls(ballsProgressData);
                _gameProcessor.Field.GenerateNextBall(
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountAfterMove, 
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallWeightsRange,
                    _gameProcessor.Scene.ActiveHats);
                
                foreach (var buffProgress in lastSessionProgress.Buffs)
                {
                    var foundBuff = _gameProcessor.Buffs.Find(i => i.Id == buffProgress.Id);
                    foundBuff.SetRestCooldown(buffProgress.RestCooldown);
                }

                _gameProcessor.CommonAnalytics.SetProgress(lastSessionProgress.Analytics);
            }
            else
            {
                _gameProcessor.CastleSelector.SelectActiveCastle(GetFirstUncompletedCastleName());
        
                ApplicationController.Instance.SaveController.SaveLastSessionProgress.Clear();
                _gameProcessor.Field.Clear();
                _gameProcessor.Field.GenerateBalls(
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountOnStart, 
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallWeightsRange, 
                    _gameProcessor.Scene.ActiveHats);
                _gameProcessor.CastleSelector.ActiveCastle.ResetPoints(true);
            }
        }
    
        private async Task ProcessSessionAsync(UIGameScreen gameScreen, CancellationToken restartToken, CancellationToken loseToken, CancellationToken cancellationToken)
        {
            _gameProcessor.Field.GenerateNextBall(
                _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountAfterMove, 
                _gameProcessor.ActiveGameRulesSettings.GeneratedBallWeightsRange,
                _gameProcessor.Scene.ActiveHats);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                restartToken.ThrowIfCancellationRequested();
                loseToken.ThrowIfCancellationRequested();

                var noAvailableSteps = false;
                if (_notAllBallsGenerated)
                {
                    _loseTokenSource.Cancel();
                }
                else
                {
                    noAvailableSteps = _gameProcessor.Field.IsFullFilled && !_gameProcessor.Field.HasMergeSteps(_gameProcessor.ActiveGameRulesSettings.GeneratedBallPointsRange);
                    if (noAvailableSteps)
                    {
                        _loseTokenSource.Cancel();
                    }
                }
            
                _userStepFinished = false;
                _notAllBallsGenerated = false;

                CheckLowEmptySpace(noAvailableSteps);
            
                if (CheckCastleCompetedState())
                {
                    await ProcessCastleCompleting(gameScreen);
                }
                else
                {
                    while (!_userStepFinished)
                    {
                        CheckAndProcessFieldEmpty();

                        cancellationToken.ThrowIfCancellationRequested();
                        restartToken.ThrowIfCancellationRequested();
                        loseToken.ThrowIfCancellationRequested();

                        await Task.Yield();
                    }
                }
            }
        }

        private bool CheckCastleCompetedState()
        {
            var activeCastle = _gameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
                return activeCastle.Completed;
            return false;
        }
    
        private async Task ProcessCastleCompleting(UIGameScreen gameScreen)
        {
            var activeCastle = _gameProcessor.CastleSelector.ActiveCastle;
        
            try
            {
                OnCastleCompleted?.Invoke(activeCastle);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
       
            ApplicationController.Instance.SaveController.SaveProgress.MarkCastleCompleted(activeCastle.Id);

            _completedCastles.Add(new CompletedCastleDesc(activeCastle.Id, activeCastle.GetPoints()));

            var dialogKeyOnBuildStarting = GuidEx.Empty;
            var nextCastle = GetFirstUncompletedCastle();
        
            if (nextCastle != null)
            {
                dialogKeyOnBuildStarting = nextCastle.TextOnBuildStartingKey;
            }
        
            gameScreen.LockInput(true);
            _gameProcessor.Field.View.LockInput(true);

            await activeCastle.WaitForPointsReceiveEffectComplete(Application.exitCancellationToken);
            await activeCastle.WaitForAnimationsComplete(Application.exitCancellationToken);

            var coinsAfterComplete = activeCastle.CoinsAfterComplete;
            await ProcessCastleCompleteAsync(
                activeCastle.TextAfterBuildEndingKey, 
                dialogKeyOnBuildStarting, 
                false, 
                null,
                null,
                null,
                async () =>
                {
                    _gameProcessor.AddCurrency(coinsAfterComplete);
                    ApplicationController.Instance.SaveController.SaveLastSessionProgress.ChangeProgress(this);
                },
                Application.exitCancellationToken);
        
            gameScreen.LockInput(false);
            _gameProcessor.Field.View.LockInput(false);
            
            _ = ApplicationController.Instance.ISocialService.SetScoreForLeaderBoard(
                BEST_SESSION_SCORE_LEADERBOARD, 
                _saveController.Value.SaveProgress.BestSessionScore, 
                Application.exitCancellationToken);
        }
    
        public async Task ProcessCastleCompleteAsync(
            GuidEx dialogKeyAfterBuildEnding,
            GuidEx dialogKeyOnBuildStarting,
            bool manualGiveCoins,
            Func<Task> beforeGiveCoins, 
            Func<Task> beforeSelectNextCastle,
            Func<Task> afterSelectNextCastle,
            Func<Task> inTheEnd,
            CancellationToken exitToken)
        {
            //wait for last animation is playing 
            await AsyncExtensions.WaitForSecondsAsync(2.0f, exitToken);
        
            var castleCompletePanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UICastleCompletePanel>(
                new UICastleCompletePanelData()
                {
                    Layer = "gameScreenFrontLayer",
                    GameProcessor = _gameProcessor, 
                    DialogAfterBuildEndingKey = dialogKeyAfterBuildEnding,
                    DialogOnBuildStartingKey = dialogKeyOnBuildStarting,
                    ManualGiveCoins = manualGiveCoins,
                    BeforeGiveCoins = beforeGiveCoins,
                    BeforeSelectNextCastle = beforeSelectNextCastle,
                    AfterSelectNextCastle = afterSelectNextCastle,
                    InTheEnd = inTheEnd,
                }, 
                exitToken);

            await castleCompletePanel.ShowAsync(exitToken);
        }

        public IReadOnlyList<ICastle> GetCompletedCastles()
        {
            return _completedCastles;
        }

        public ICastle GetActiveCastle()
        {
            return _gameProcessor.CastleSelector.ActiveCastle;
        }

        public IField GetField()
        {
            return _gameProcessor.Field;
        }

        public IEnumerable<IBuff> GetBuffs()
        {
            return _gameProcessor.Buffs;
        }

        public string GetFirstUncompletedCastleName()
        {
            var firstUncompletedCastle = GetFirstUncompletedCastle();
            return firstUncompletedCastle.Id;
        }
    
        public Castle GetFirstUncompletedCastle()
        {
            var firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.FirstOrDefault(i => !ApplicationController.Instance.SaveController.SaveProgress.IsCastleCompleted(i.Id));
            if (firstUncompletedCastle == null)
                firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.Last();

            return firstUncompletedCastle;
        }

        public void SelectNextCastle()
        {
            _gameProcessor.CastleSelector.SelectActiveCastle(GetFirstUncompletedCastleName());
        }
    
        public ICommonAnalytics GetCommonAnalyticsAnalytics()
        {
            return _gameProcessor.CommonAnalytics;
        }

        public int GetMergedBallsCount()
        {
            return _mergedBallsCount;
        }

        public int GetCollapseLinesCount()
        {
            return _collapsedLinesCount;
        }

        private void CheckLowEmptySpace(bool noAvailableSteps)
        {
            var emptyCellsCount = _gameProcessor.Field.CalculateEmptySpacesCount();
            var threshold = Mathf.Max(
                _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountAfterMerge,
                _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountAfterMove);
            var lowSpace = emptyCellsCount <= threshold;
            var freeSpaceIsOver = emptyCellsCount <= 0;

            OnLowEmptySpaceChanged?.Invoke(lowSpace);
            OnFreeSpaceIsOverChanged?.Invoke(freeSpaceIsOver, noAvailableSteps);
        }
    
        private void CheckAndProcessFieldEmpty()
        {
            if (_gameProcessor.Field.IsEmpty)
            {
                _gameProcessor.Field.GenerateBalls(
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallsCountOnStart, 
                    _gameProcessor.ActiveGameRulesSettings.GeneratedBallWeightsRange, 
                    _gameProcessor.Scene.ActiveHats);
            
                _gameProcessor.ClearUndoSteps();
            }
        }
    
        public bool HasPreviousSessionGame
        {
            get
            {
                var lastSessionProgress = ApplicationController.Instance.SaveController.SaveLastSessionProgress;
                return lastSessionProgress.IsValid();
            }
        }

        internal void TriggerUserStepFinished()
        {
            _userStepFinished = true;
        }

        internal void TriggerStepFinished(Step step)
        {
            if (!NonSaveTags.Contains(step.Tag))
            {
                ApplicationController.Instance.SaveController.SaveLastSessionProgress.ChangeProgress(this);
            }
        }
    
        internal void SetNotAllBallsGenerated(bool state)
        {
            _notAllBallsGenerated = state;
        }
    
        public void RestartSession()
        {
            _restartTokenSource.Cancel();
        }

        public int GetScore()
        {
            var score = 0;
            score += _completedCastles.Sum(i => i.GetPoints());
            score += _gameProcessor.CastleSelector.ActiveCastle.GetPoints();

            return score;
        }
    
        public void ChangeMergeCount(int delta)
        {
            _mergedBallsCount += delta;
        }

        public void ChangeCollapseLinesCount(int delta)
        {
            _collapsedLinesCount += delta;
        }
        
        public class CompletedCastleDesc : ICastle
        {
            public string Id { get; }
            private int Points { get; }
            public bool Completed { get; }

            public CompletedCastleDesc(string id, int points)
            {
                Id = id;
                Points = points;
            }
        
            public int GetPoints()
            {
                return Points;
            }
        }
    }
}