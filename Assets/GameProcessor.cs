using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Buffs;
using Core.Effects;
using Core.Goals;
using Core.Steps;
using Core.Steps.CustomOperations;
using Core.Tutorials;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IRules
{
    int MinimalBallsInLine { get; }
}

public interface IPointsChangeListener
{
    void AddPoints(int points);
    void RemovePoints(int points);
}

public enum ExplodeType
{
    Explode1,
    Explode3,
    ExplodeHorizontal,
    ExplodeVertical,
}

public enum StepTag
{
    Move,
    Merge,
    Explode1,
    Explode3,
    ExplodeHorizontal,
    ExplodeVertical,
    NextBalls,
    Downgrade,
    
    Undo,
    Select,
    Deselect,
    ChangeSelected,
    NoPath,
    
    UndoMove,
    UndoMerge,
    UndoExplode1,
    UndoExplode3,
    UndoExplodeHorizontal,
    UndoExplodeVertical,
    UndoNextBalls,
    UndoDowngrade,
    
    None,
}

public class GameProcessor : MonoBehaviour, IRules, IPointsChangeListener, ISessionProgressHolder
{
    public static readonly List<StepTag> NewStepStepTags = new()
    {
        { StepTag.Move },
        { StepTag.Merge },
    };

    public static readonly Dictionary<StepTag, StepTag> UndoStepTags = new()
    {
        { StepTag.Move, StepTag.UndoMove },
        { StepTag.Merge, StepTag.UndoMerge },
        { StepTag.Explode1, StepTag.UndoExplode1 },
        { StepTag.Explode3, StepTag.UndoExplode3 },
        { StepTag.ExplodeHorizontal, StepTag.UndoExplodeHorizontal },
        { StepTag.ExplodeVertical, StepTag.UndoExplodeVertical },
        { StepTag.NextBalls, StepTag.UndoNextBalls },
        { StepTag.Downgrade, StepTag.UndoDowngrade },
    };

    public static readonly Dictionary<ExplodeType, StepTag> ExplodeTypeToStepTags = new()
    {
        { ExplodeType.Explode1, StepTag.Explode1 },
        { ExplodeType.Explode3, StepTag.Explode3 },
        { ExplodeType.ExplodeHorizontal, StepTag.ExplodeHorizontal },
        { ExplodeType.ExplodeVertical, StepTag.ExplodeVertical },
    };

    public event Action<Step, StepExecutionType> OnStepCompleted;
    public event Action<Step, StepExecutionType> OnBeforeStepStarted;
    public event Action OnUndoStepsClear;
    
    public event Action<int> OnScoreChanged;
    public event Action<bool> OnLowEmptySpaceChanged;

    [SerializeField] private Scene _scene;
    [SerializeField] private Field _field;
    [SerializeField] private StepMachine _stepMachine;
    [SerializeField] private PlayerInfo _playerInfo;
    [SerializeField] private DefaultMarket _market;

    [SerializeField] private DestroyBallEffect _destroyBallEffectPrefab;
    [SerializeField] private NoPathEffect _noPathEffectPrefab;
    [SerializeField] private CollapsePointsEffect _collapsePointsEffectPrefab;

    [SerializeField] private int _minimalBallsInLine = 5;
    [SerializeField] private int _generatedBallsCountAfterMerge = 2;
    [SerializeField] private int _generatedBallsCountAfterMove = 3;
    [SerializeField] private int _generatedBallsCountOnStart = 5;
    [SerializeField] private Vector2Int _generatedBallsPointsRange = new Vector2Int(0, 10);

    [SerializeField] private RectTransform _uiScreensRoot;

    [SerializeField] private List<Buff> _buffs;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;
    [SerializeField] private CastleSelector _castleSelector;
    //todo extract
    [SerializeField] private GiveCoinsEffect _giveCoinsEffect;
    public GiveCoinsEffect GiveCoinsEffect => _giveCoinsEffect;

    private Ball _selectedBall;
    private Ball _otherSelectedBall;
    private PointsCalculator _pointsCalculator;
    private int _score;
    private bool _userStepFinished = false;
    private bool _notAllBallsGenerated = false;
    private int _bestSessionScore;
    private CancellationTokenSource _cancellationTokenSource;

    public Scene Scene => _scene;
    public PlayerInfo PlayerInfo => _playerInfo;
    public int Score => _score;
    public IMarket Market => _market;
    public PurchasesLibrary PurchasesLibrary => _purchasesLibrary;
    public CastleSelector CastleSelector => _castleSelector;

    public int BestSessionScore
    {
        get => _bestSessionScore;
    }

    private void Awake()
    {
        _pointsCalculator = new PointsCalculator(this);
        _cancellationTokenSource = new CancellationTokenSource();
        
        _market.OnBought += Market_OnBought;
    }

    private void Market_OnBought(bool result, string productId)
    {
        var purchaseItem = _purchasesLibrary.Items.Find(i => string.Equals(i.ProductId, productId, StringComparison.Ordinal));
       
        if (result && purchaseItem != null)
        {
            _playerInfo.AddCoins(purchaseItem.CurrencyAmount);
        }
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private async void Start()
    {
        _field.OnPointerDown += Field_OnPointerDown;
        _stepMachine.OnBeforeStepStarted += StepMachine_OnBeforeStepStarted;
        _stepMachine.OnStepCompleted += StepMachine_OnStepCompleted;
        _stepMachine.OnUndoStepsClear += StepMachine_OnUndoStepsClear;

        ApplicationController.Instance.UIPanelController.SetScreensRoot(_uiScreensRoot);

        Load();
        
        _castleSelector.Init();
        
        await ProcessGame(_cancellationTokenSource.Token);
    }

    private async Task ProcessGame(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            await PrepareSession();
            await ProcessSession();
        }
    }
    
    private async Task PrepareSession()
    {
        _bestSessionScore = PlayerInfo.GetBestSessionScore();
        
        
        if (_tutorialController.CanStartTutorial(_forceTutorial))
        {
            await StartTutorial(_forceTutorial);
        }
        else
        {
            _castleSelector.OnCastleCompleted += CastleSelector_OnCastleCompleted;

            if (HasPreviousSessionGame)
            {
                var lastSessionProgress = PlayerInfo.GetLastSessionProgress();
                _score = lastSessionProgress.Score;

                _castleSelector.SelectActiveCastle(lastSessionProgress.Castle.Id);
                _castleSelector.ActiveCastle.SetPoints(lastSessionProgress.Castle.Points);
            
                var ballsProgressData = lastSessionProgress.Field.Balls.Select(i => (i.GridPosition, i.Points));
                _field.AddBalls(ballsProgressData);

                foreach (var buffProgress in lastSessionProgress.Buffs)
                {
                    var foundBuff = _buffs.Find(i => i.Id == buffProgress.Id);
                    foundBuff.SetRestCooldown(buffProgress.RestCooldown);
                }
            }
            else
            {
                _castleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
            
                _playerInfo.ClearLastSessionProgress();
                _field.Clear();
                _field.GenerateBalls(_generatedBallsCountOnStart, _generatedBallsPointsRange);
                _castleSelector.ActiveCastle.ResetPoints();
            }
            
            await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIGameScreen), 
                new UIGameScreenData() { GameProcessor = this }, 
                _cancellationTokenSource.Token);
        
            var startPanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIStartPanel), 
                new UIStartPanelData() { GameProcessor = this }, 
                _cancellationTokenSource.Token) as UIStartPanel;
            await startPanel.Showing(_cancellationTokenSource.Token);

            if (startPanel.Choice == UIStartPanelChoice.New)
            {
                _castleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
            
                _playerInfo.ClearLastSessionProgress();
                _field.Clear();
                _field.GenerateBalls(_generatedBallsCountOnStart, _generatedBallsPointsRange);
                _castleSelector.ActiveCastle.ResetPoints();
            }
        }
    }

    public bool TutorialNotCompleted => true;

    [SerializeField] private bool _forceTutorial;
    [SerializeField] private TutorialController _tutorialController;
    private async Task StartTutorial(bool forceTutorial)
    {
        await _tutorialController.TryStartTutorial(forceTutorial, _cancellationTokenSource.Token);
    }

    
    private void Load()
    {
        _playerInfo.Load();
    }
    
    private async Task ProcessSession()
    {
        _field.GenerateNextBallPositions(_generatedBallsCountAfterMove, _generatedBallsPointsRange);

        while (true)
        {
            if (!_field.IsEmpty && _notAllBallsGenerated) break;

            _userStepFinished = false;
            _notAllBallsGenerated = false;

            while (!_userStepFinished)
                await Task.Yield();

            CheckLowEmptySpace();
        }

        var failPanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
            typeof(UIGameFailPanel), 
            new UIGameFailPanelData() { GameProcessor = this }, 
            _cancellationTokenSource.Token);

        await failPanel.Showing(_cancellationTokenSource.Token);
    }
    
    private async void CastleSelector_OnCastleCompleted()
    {
        ProcessCastleComplete(null, null, null);
    }

    public async Task ProcessCastleComplete(
        Func<Task> beforeGiveCoins, 
        Func<Task> beforeSelectNextCastle,
        Func<Task> afterSelectNextCastle)
    {
        var castleCompletePanel = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
            typeof(UICastleCompletePanel),
            new UICastleCompletePanel.UICastleCompletePanelData()
            {
                GameProcessor = this, 
                BeforeGiveCoins = beforeGiveCoins,
                BeforeSelectNextCastle = beforeSelectNextCastle,
                AfterSelectNextCastle = afterSelectNextCastle,
            }, 
            _cancellationTokenSource.Token);

        await castleCompletePanel.Showing(_cancellationTokenSource.Token);
        
        SelectNextCastle();
    }
    
    void Field_OnPointerDown(Vector3Int pointerGridPosition)
    {
        var balls = _field.GetSomething<Ball>(pointerGridPosition).ToList();
        Ball ball = null;
        if (balls.Count != 0)
            ball = balls[0];

        _otherSelectedBall = null;

        if (_selectedBall != null)
        {
            if (ball != null)
            {
                if (_selectedBall == ball)
                {
                    _stepMachine.AddStep(new Step(StepTag.Deselect,
                        new SelectOperation(pointerGridPosition, false, _field)
                            .SubscribeCompleted(OnDeselectBall)));
                }
                else
                {
                    if (_selectedBall.Points == ball.Points)
                    {
                        _otherSelectedBall = ball;
                        var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                        if (path.Count > 0)
                        {
                            MergeStep(_selectedBall.IntGridPosition, pointerGridPosition);
                        }
                        else
                        {
                            _stepMachine.AddStep(new Step(StepTag.NoPath,
                                new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition,
                                    _noPathEffectPrefab, _field)));
                        }
                    }
                    else
                    {
                        _stepMachine.AddStep(new Step(StepTag.ChangeSelected,
                            new SelectOperation(_selectedBall.IntGridPosition, false, _field).SubscribeCompleted(
                                OnDeselectBall),
                            new SelectOperation(pointerGridPosition, true, _field).SubscribeCompleted(OnSelectBall)));
                    }
                }
            }
            else
            {
                var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                if (path.Count > 0)
                {
                    MoveStep(_selectedBall.IntGridPosition, pointerGridPosition);
                }
                else
                {
                    _stepMachine.AddStep(new Step(StepTag.NoPath,
                        new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition,
                            _noPathEffectPrefab, _field)));
                }
            }
        }
        else
        {
            if (ball != null)
            {
                _stepMachine.AddStep(new Step(StepTag.Select, new SelectOperation(pointerGridPosition, true, _field)
                    .SubscribeCompleted(OnSelectBall)));
            }
        }
    }

    private void MergeStep(Vector3Int from, Vector3Int to)
    {
        _stepMachine.AddStep(new Step(StepTag.Merge,
            new MoveOperation(from, to, _field),
            new MergeOperation(to, _field),
            new SelectOperation(to, false, _field)
                .SubscribeCompleted(OnDeselectBall),
            new CollapseOperation(to, _collapsePointsEffectPrefab,
                _destroyBallEffectPrefab, _field, _pointsCalculator, this),
            new CheckIfGenerationIsNecessary(
                null,
                new List<Operation>()
                {
                    new GenerateOperation(_generatedBallsCountAfterMerge,
                        _generatedBallsCountAfterMove, _generatedBallsPointsRange, _field),
                    new CollapseOperation(_collapsePointsEffectPrefab, _destroyBallEffectPrefab,
                        _field, _pointsCalculator, this)
                })));
    }

    private void MoveStep(Vector3Int from, Vector3Int to)
    {
        _stepMachine.AddStep(new Step(StepTag.Move,
            new MoveOperation(from, to, _field),
            new SelectOperation(to, false, _field)
                .SubscribeCompleted(OnDeselectBall),
            new CollapseOperation(to, _collapsePointsEffectPrefab,
                _destroyBallEffectPrefab, _field, _pointsCalculator, this),
            new CheckIfGenerationIsNecessary(
                null,
                new List<Operation>()
                {
                    new GenerateOperation(_generatedBallsCountAfterMove, _generatedBallsCountAfterMove,
                        _generatedBallsPointsRange, _field),
                    new CollapseOperation(_collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field,
                        _pointsCalculator, this)
                })));
    }

    private void OnSelectBall(Operation sender, object result)
    {
        _selectedBall = (Ball)result;
    }

    private void OnDeselectBall(Operation sender, object result)
    {
        _selectedBall = null;
    }

    private void StepMachine_OnBeforeStepStarted(Step step, StepExecutionType executionType)
    {
        OnBeforeStepStarted?.Invoke(step, executionType);

        if (NewStepStepTags.Contains(step.Tag))
            foreach (var buff in _buffs)
                if (buff.RestCooldown != 0)
                    step.AddOperations(new List<Operation>() { new ProcessBuffCooldownOperation(buff) });
    }

    private void CheckLowEmptySpace()
    {
        var emptyCellsCount = _field.CalculateEmptySpacesCount();
        var threshold = Mathf.Max(_generatedBallsCountAfterMerge, _generatedBallsCountAfterMove);
        bool lowSpace = emptyCellsCount <= threshold;

        OnLowEmptySpaceChanged?.Invoke(lowSpace);
    }

    private void StepMachine_OnStepCompleted(Step step, StepExecutionType executionType)
    {
        if (UndoStepTags.ContainsKey(step.Tag))
        {
            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(UndoStepTags[step.Tag], inverseOperations));

            var generateOperationData = step.GetData<GenerateOperationData>();
            if (generateOperationData != null)
                _notAllBallsGenerated = generateOperationData.NewBallsData.Count < generateOperationData.RequiredAmount;
            _userStepFinished = true;
        }

        _playerInfo.SaveSessionProgress(this);
        
        OnStepCompleted?.Invoke(step, executionType);
    }

    private void StepMachine_OnUndoStepsClear()
    {
        OnUndoStepsClear?.Invoke();
    }


    public bool HasUndoSteps()
    {
        return _stepMachine.HasUndoSteps();
    }

    public int MinimalBallsInLine => _minimalBallsInLine;
    public List<Buff> Buffs => _buffs;

    public bool HasPreviousSessionGame
    {
        get
        {
            var lastSessionProgress = _playerInfo.GetLastSessionProgress();
            return lastSessionProgress != null && lastSessionProgress.IsValid();
        }
    }


    public void AddPoints(int points)
    {
        _score += points;
        
        PlayerInfo.SetBestSessionScore(_score);
        OnScoreChanged?.Invoke(points);
    }

    public void RemovePoints(int points)
    {
        _score -= points;
        
        PlayerInfo.SetBestSessionScore(_score);
        
        OnScoreChanged?.Invoke(-points);
    }
    
    public void UseUndoBuff(int cost, UndoBuff buff)
    {
        _stepMachine.AddStep(
            new Step(StepTag.Undo,
                new SpendOperation(cost, _playerInfo, false),
                new UndoOperation(_stepMachine),
                new ConfirmBuffUseOperation(buff)));
    }

    public void UseExplodeBuff(int cost, ExplodeType explodeType, List<Vector3Int> ballsIndexes, ExplodeBuff buff)
    {
        _stepMachine.AddStep(
            new Step(ExplodeTypeToStepTags[explodeType], 
                new SpendOperation(cost, _playerInfo, true),
                new RemoveOperation(ballsIndexes, _field),
                new ConfirmBuffUseOperation(buff)));
    }

    public void UseShowNextBallsBuff(int cost, INextBallsShower nextBallsShower, ShowNextBallsBuff buff)
    {
        _stepMachine.AddStep(
            new Step(StepTag.NextBalls, 
                new SpendOperation(cost, _playerInfo, true),
                new NextBallsShowOperation(true, nextBallsShower),
                new ConfirmBuffUseOperation(buff)));
    }

    public bool CanGradeAny(IEnumerable<Vector3Int> ballsIndexes)
    {
        var gradeLevel = -1;
        var balls = ballsIndexes.SelectMany(i => _field.GetSomething<Ball>(i)).ToList();
        return balls.Any(ball => ball.CanGrade(gradeLevel));
    }
    
    public void UseDowngradeBuff(int cost, List<Vector3Int> ballsIndexes, DowngradeBuff buff)
    {
        var gradeLevel = -1;
        _stepMachine.AddStep(
            new Step(StepTag.Downgrade,
                new SpendOperation(cost, _playerInfo, true),
                new GradeOperation(ballsIndexes, gradeLevel, _field),
                new ConfirmBuffUseOperation(buff),
                new CollapseOperation(ballsIndexes[0], _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this)));
    }

    public void SelectNextCastle()
    {
        _castleSelector.SelectActiveCastle(GetFirstUncompletedCastle());
    }

    public void ClearUndoSteps()
    {
        _stepMachine.ClearUndoSteps();
    }

    public ICastle GetCastle()
    {
        return _castleSelector.ActiveCastle;
    }

    public IField GetField()
    {
        return _field;
    }

    public IEnumerable<IBuff> GetBuffs()
    {
        return _buffs;
    }

    public int GetScore()
    {
        return _score;
    }

    public string GetFirstUncompletedCastle()
    {
        var firstUncompletedCastle = _castleSelector.Library.Castles.FirstOrDefault(i => !_playerInfo.IsCastleCompleted(i.Id));
        if (firstUncompletedCastle == null)
            firstUncompletedCastle = _castleSelector.Library.Castles.Last();

        return firstUncompletedCastle.Id;
    }

    public void SelectBall(Vector3Int gridPosition)
    {
        _stepMachine.AddStep(new Step(StepTag.Select, new SelectOperation(gridPosition, true, _field)
            .SubscribeCompleted(OnSelectBall)));
    }
    public async Task MoveBall(Vector3Int from, Vector3Int to, CancellationToken cancellationToken)
    {
        bool stepCompleted = false;
        
        _stepMachine.OnStepCompleted += StepCompleted;
        MoveStep(from, to);
        
        while (!stepCompleted)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
        }
        
        void StepCompleted(Step arg1, StepExecutionType arg2)
        {
            stepCompleted = true;
        }
    }

    public async Task MergeBall(Vector3Int from, Vector3Int to, CancellationToken cancellationToken)
    {
        bool stepCompleted = false;

        _stepMachine.OnStepCompleted += StepCompleted;
        MergeStep(from, to);

        while (!stepCompleted)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
        }

        void StepCompleted(Step arg1, StepExecutionType arg2)
        {
            stepCompleted = true;
        }
    }

    public async Task GiveTutorialCoins(int coinsAmount)
    {
        
    }
}