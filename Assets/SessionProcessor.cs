using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analytics;
using Atom;
using Core;
using Core.Goals;
using Save;
using UnityEngine;

public class SessionProcessor : MonoBehaviour, 
    ISessionProgressHolder
{
    public event Action OnLose;
    public event Action OnRestart;
    public event Action<int> OnScoreChanged;
    public event Action<bool> OnLowEmptySpaceChanged;
    public event Action<bool> OnFreeSpaceIsOverChanged; 
    
    [SerializeField] private bool _enableTutorial;
    [SerializeField] private bool _forceTutorial;
    
    private CancellationTokenSource _restartTokenSource;
    private CancellationTokenSource _loseTokenSource;
    private readonly List<CompletedCastleDesc> _completedCastles = new();
    
    private GameProcessor _gameProcessor;
    private DependencyHolder<UIPanelController> _panelController;
   
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
            _gameProcessor.Scene.SetSkin(activeSkinName);
            var activeHatName = ApplicationController.Instance.SaveController.SaveSettings.ActiveHat;
            _gameProcessor.Scene.SetHat(activeHatName);
            
            if (_enableTutorial && _gameProcessor.TutorialController.CanStartTutorial(_forceTutorial))
            {
                await _gameProcessor.TutorialController.TryStartTutorial(_forceTutorial, exitToken);
            }
            else
            {
                _gameProcessor.MusicPlayer.PlayNext();
                
                var startPanel = await _panelController.Value.PushScreenAsync<UIStartPanel>(
                    new UIStartPanelData()
                    {
                        GameProcessor = _gameProcessor,
                        Instant = false,
                    },
                    exitToken);
                
                await startPanel.ShowAsync(exitToken);
                
                PrepareSession();
                
                await _panelController.Value.PushPopupScreenAsync<UIGameScreen>(
                    new UIGameScreenData()
                    {
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
                    await ProcessSessionAsync(restartToken, loseToken, exitToken);
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
                        
                        ApplicationController.Instance.SaveController.SaveProgress.SetBestSessionScore(GetScore());
                        ApplicationController.Instance.SaveController.SaveLastSessionProgress.Clear();

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
        
                        var failPanel = await _panelController.Value.PushPopupScreenAsync<UIGameFailPanel>(
                            new UIGameFailPanelData() { GameProcessor = _gameProcessor }, 
                            exitToken);

                        await failPanel.ShowAsync(exitToken);
                        
                        ApplicationController.Instance.SaveController.SaveProgress.SetBestSessionScore(GetScore());
                        ApplicationController.Instance.SaveController.SaveLastSessionProgress.Clear();
        
                        _gameProcessor.MusicPlayer.PlayNext();
                        
                        var startPanel1 = await _panelController.Value.PushScreenAsync<UIStartPanel>(
                            new UIStartPanelData()
                            {
                                GameProcessor = _gameProcessor,
                                Instant = true,
                            },
                            exitToken);
                        await startPanel1.ShowAsync(exitToken);
                        
                        PrepareSession();
                        await _panelController.Value.PushPopupScreenAsync<UIGameScreen>(
                            new UIGameScreenData() { GameProcessor = _gameProcessor },
                            exitToken);
                    }
                    else if (exception.CancellationToken == exitToken)
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
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
        _gameProcessor.CastleSelector.OnCastleCompleted -= CastleSelector_OnCastleCompleted;
        _gameProcessor.CastleSelector.OnCastleCompleted += CastleSelector_OnCastleCompleted;
        
        if (HasPreviousSessionGame)
        {
            var lastSessionProgress = ApplicationController.Instance.SaveController.SaveLastSessionProgress;
            
            _completedCastles.AddRange(lastSessionProgress.CompletedCastles.Select(i=>new CompletedCastleDesc(i.Id, i.Points)));
            _gameProcessor.CastleSelector.SelectActiveCastle(lastSessionProgress.ActiveCastle.Id);
            _gameProcessor.CastleSelector.ActiveCastle.SetPoints(lastSessionProgress.ActiveCastle.Points, true);
        
            var ballsProgressData = lastSessionProgress.Field.Balls.Select(i => new BallDesc(i.GridPosition, i.Points));
            _gameProcessor.Field.AddBalls(ballsProgressData);

            foreach (var buffProgress in lastSessionProgress.Buffs)
            {
                var foundBuff = _gameProcessor.Buffs.Find(i => i.Id == buffProgress.Id);
                foundBuff.SetRestCooldown(buffProgress.RestCooldown);
            }

            _gameProcessor.CommonAnalytics.SetProgress(lastSessionProgress.Analytics);
        }
        else
        {
            _gameProcessor.CastleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
        
            ApplicationController.Instance.SaveController.SaveLastSessionProgress.Clear();
            _gameProcessor.Field.Clear();
            _gameProcessor.Field.GenerateBalls(_gameProcessor.GeneratedBallsCountOnStart, _gameProcessor.GeneratedBallsPointsRange);
            _gameProcessor.CastleSelector.ActiveCastle.ResetPoints(true);
        }
    }
    
    private async Task ProcessSessionAsync(CancellationToken restartToken, CancellationToken loseToken, CancellationToken cancellationToken)
    {
        _gameProcessor.Field.GenerateNextBallPositions(_gameProcessor.GeneratedBallsCountAfterMove, _gameProcessor.GeneratedBallsPointsRange);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            restartToken.ThrowIfCancellationRequested();
            loseToken.ThrowIfCancellationRequested();

            if (!_gameProcessor.Field.IsEmpty && _notAllBallsGenerated)
            {
                _loseTokenSource.Cancel();
            }
            
            _userStepFinished = false;
            _notAllBallsGenerated = false;

            while (!_userStepFinished)
            {
                cancellationToken.ThrowIfCancellationRequested();
                restartToken.ThrowIfCancellationRequested();
                loseToken.ThrowIfCancellationRequested();

                await Task.Yield();
            }

            CheckLowEmptySpace();
        }
    }
    
    private async void CastleSelector_OnCastleCompleted(Castle castle)
    {
        ApplicationController.Instance.SaveController.SaveProgress.MarkCastleCompleted(castle.Id);

        _completedCastles.Add(new CompletedCastleDesc(castle.Id, castle.GetPoints()));
        
        _ = ProcessCastleCompleteAsync(GuidEx.Empty, null, null, null, Application.exitCancellationToken);
    }
    
    public async Task ProcessCastleCompleteAsync(
        GuidEx dialogTextKey,
        Func<Task> beforeGiveCoins, 
        Func<Task> beforeSelectNextCastle,
        Func<Task> afterSelectNextCastle,
        CancellationToken exitToken)
    {
        //wait for last animation is playing 
        await AsyncExtensions.WaitForSecondsAsync(2.0f, exitToken);
        
        var castleCompletePanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UICastleCompletePanel>(
            new UICastleCompletePanelData()
            {
                GameProcessor = _gameProcessor, 
                DialogTextKey = dialogTextKey,
                BeforeGiveCoins = beforeGiveCoins,
                BeforeSelectNextCastle = beforeSelectNextCastle,
                AfterSelectNextCastle = afterSelectNextCastle,
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

    public string GetFirstUncompletedCastle()
    {
        var firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.FirstOrDefault(i => !ApplicationController.Instance.SaveController.SaveProgress.IsCastleCompleted(i.Id));
        if (firstUncompletedCastle == null)
            firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.Last();

        return firstUncompletedCastle.Id;
    }

    public void SelectNextCastle()
    {
        _gameProcessor.CastleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
    }
    
    public ICommonAnalytics GetCommonAnalyticsAnalytics()
    {
        return _gameProcessor.CommonAnalytics;
    }

    private void CheckLowEmptySpace()
    {
        var emptyCellsCount = _gameProcessor.Field.CalculateEmptySpacesCount();
        var threshold = Mathf.Max(_gameProcessor.GeneratedBallsCountAfterMerge, _gameProcessor.GeneratedBallsCountAfterMove);
        var lowSpace = emptyCellsCount <= threshold;
        var freeSpaceIsOver = emptyCellsCount <= 0;

        OnLowEmptySpaceChanged?.Invoke(lowSpace);
        OnFreeSpaceIsOverChanged?.Invoke(freeSpaceIsOver);
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