using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core;
using Core.Goals;
using UnityEngine;

public class SessionProcessor : MonoBehaviour, IPointsChangeListener
{
    public event Action OnLose;
    public event Action OnRestart;
    public event Action<int> OnScoreChanged;
    public event Action<bool> OnLowEmptySpaceChanged;
    public event Action<bool> OnFreeSpaceIsOverChanged; 
    
    [SerializeField] private bool _enableTutorial;
    [SerializeField] private bool _forceTutorial;
    
    // private GameProcessor _gameProcessor;
    // private List<string> _completedCastle = new List<string>();
    //
    // public void SetData(GameProcessor gameProcessor)
    // {
    //     _gameProcessor = gameProcessor;
    // }
    //     
    // public int GetEarnedPoints()
    // {
    //     var earnedPoints = 0;
    //     
    //     if (_gameProcessor._castleSelector.ActiveCastle != null)
    //     {
    //         earnedPoints += _gameProcessor._castleSelector.ActiveCastle.GetPoints();
    //     }
    //
    //     return earnedPoints;
    // }
    //     
    // public void Reset()
    // {
    //     //_gameProcessor._castleSelector.
    //     //_points = 0;
    //     //_completed = ProcessPoints(0, instant);
    // }

    private CancellationTokenSource _restartTokenSource;
    private CancellationTokenSource _loseTokenSource;
    
    private GameProcessor _gameProcessor;
    private DependencyHolder<UIPanelController> _panelController;
   
    private bool _userStepFinished = false;
    private bool _notAllBallsGenerated = false;
    private int _score;
    
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
            _score = lastSessionProgress.Score;

            _gameProcessor.CastleSelector.SelectActiveCastle(lastSessionProgress.Castle.Id);
            _gameProcessor.CastleSelector.ActiveCastle.SetPoints(lastSessionProgress.Castle.Points, true);
        
            var ballsProgressData = lastSessionProgress.Field.Balls.Select(i => new BallDesc(i.GridPosition, i.Points));
            _gameProcessor.GetField().AddBalls(ballsProgressData);

            foreach (var buffProgress in lastSessionProgress.Buffs)
            {
                var foundBuff = _gameProcessor.Buffs.Find(i => i.Id == buffProgress.Id);
                foundBuff.SetRestCooldown(buffProgress.RestCooldown);
            }

            _gameProcessor.GetCommonAnalyticsAnalytics().SetProgress(lastSessionProgress.Analytics);
        }
        else
        {
            _gameProcessor.CastleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
        
            ApplicationController.Instance.SaveController.SaveLastSessionProgress.Clear();
            _gameProcessor.GetField().Clear();
            _gameProcessor.GetField().GenerateBalls(_gameProcessor.GeneratedBallsCountOnStart, _gameProcessor.GeneratedBallsPointsRange);
            _gameProcessor.CastleSelector.ActiveCastle.ResetPoints(true);
        }
    }
    
    private async Task ProcessSessionAsync(CancellationToken restartToken, CancellationToken loseToken, CancellationToken cancellationToken)
    {
        _gameProcessor.GetField().GenerateNextBallPositions(_gameProcessor.GeneratedBallsCountAfterMove, _gameProcessor.GeneratedBallsPointsRange);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            restartToken.ThrowIfCancellationRequested();
            loseToken.ThrowIfCancellationRequested();

            if (!_gameProcessor.GetField().IsEmpty && _notAllBallsGenerated)
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
    
    public string GetFirstUncompletedCastle()
    {
        var firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.FirstOrDefault(i => !ApplicationController.Instance.SaveController.SaveProgress.IsCastleCompleted(i.Id));
        if (firstUncompletedCastle == null)
            firstUncompletedCastle = _gameProcessor.CastleSelector.Library.Castles.Last();

        return firstUncompletedCastle.Id;
    }
    
    private void CheckLowEmptySpace()
    {
        var emptyCellsCount = _gameProcessor.GetField().CalculateEmptySpacesCount();
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

    public void AddPoints(int points)
    {
        _score += points;
        
        ApplicationController.Instance.SaveController.SaveProgress.SetBestSessionScore(_score);
        OnScoreChanged?.Invoke(points);
    }

    public void RemovePoints(int points)
    {
        _score -= points;
        
        ApplicationController.Instance.SaveController.SaveProgress.SetBestSessionScore(_score);
        
        OnScoreChanged?.Invoke(-points);
    }

    public void RestartSession()
    {
        _restartTokenSource.Cancel();
    }
}