using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Effects;
using Core.Steps;
using Core.Steps.CustomOperations;
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

public class GameProcessor : MonoBehaviour, IRules, IPointsChangeListener
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
    
    public static readonly Dictionary<ExplodeType, StepTag> ExplodeTypeToStepTags = new ()
    {
        { ExplodeType.Explode1, StepTag.Explode1 },
        { ExplodeType.Explode3, StepTag.Explode3 },
        { ExplodeType.ExplodeHorizontal, StepTag.ExplodeHorizontal },
        { ExplodeType.ExplodeVertical, StepTag.ExplodeVertical },
    };
    
    public event Action<Step, StepExecutionType> OnStepCompleted;
    public event Action<Step, StepExecutionType> OnStepExecute;
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
    
    [SerializeField] private ParticleSystem _castleOpenEffect;

    private Ball _selectedBall;
    private Ball _otherSelectedBall;
    private PointsCalculator _pointsCalculator;
    private int _score;
    private bool _userStepFinished = false;
    private bool _notAllBallsGenerated = false;

    
    public Scene Scene => _scene;
    public PlayerInfo PlayerInfo => _playerInfo;
    public int Score => _score;
    public IMarket Market => _market;
    public PurchasesLibrary PurchasesLibrary => _purchasesLibrary;
    public CastleSelector CastleSelector => _castleSelector;
    
    void Awake()
    {
        _pointsCalculator = new PointsCalculator(this);
    }
    
    private void Start()
    {
        _field.OnPointerDown += Field_OnPointerDown;
        _stepMachine.OnStepExecute += StepMachine_OnStepExecute;
        _stepMachine.OnStepCompleted += StepMachine_OnStepCompleted;

        ApplicationController.Instance.UIPanelController.SetScreensRoot(_uiScreensRoot);

        Load();
        SetDefaults();
        Init();
        
        ApplicationController.Instance.UIPanelController.PushScreen(typeof(UIStartPanel), new UIStartPanelData(){GameProcessor = this}, UIStartPanel_OnScreenReady);
    }

    private void Init()
    {
        _castleSelector.Init();
        _castleSelector.OnCastleCompleted += CastleSelector_OnCastleCompleted;
    }

    private void UIStartPanel_OnScreenReady(UIPanel sender)
    {
        var startPanel = sender as UIStartPanel;
        startPanel.OnStartPlay += () =>  StartCoroutine(InnerProcess());
    }
    
    private void Load()
    {
        _playerInfo.Load();
    }
    
    private void SetDefaults()
    {
        var lastSelectedCastle = _playerInfo.GetLastSelectedCastle();
        if (string.IsNullOrEmpty(lastSelectedCastle))
        {
            _playerInfo.SelectCastle(_castleSelector.Library.Castles[0].Name);
            lastSelectedCastle = _playerInfo.GetLastSelectedCastle();
        }

        var foundCastle = _castleSelector.Library.Castles.FirstOrDefault(i => i.Name == lastSelectedCastle);
        if (foundCastle == null)
        {
            foreach (var castle in _castleSelector.Library.Castles)
            {
                var castleProgress = _playerInfo.GetCastleProgress(castle.Name);
                if(castleProgress == null || !castleProgress.IsCompleted)
                    _playerInfo.SelectCastle(castle.Name);
            }
        }
    } 
        
    IEnumerator InnerProcess()
    {
        var waitForGameScreenReady = new PushAndWaitForScreenReady<UIGameScreen>(new UIGameScreenData() { GameProcessor = this });
        yield return waitForGameScreenReady;
        
        _field.GenerateBalls(_generatedBallsCountOnStart, _generatedBallsPointsRange);
        _field.GenerateNextBallPositions(_generatedBallsCountAfterMove, _generatedBallsPointsRange);
        
        while (true)
        {
            if(!_field.IsEmpty && _notAllBallsGenerated) break;
            
            _userStepFinished = false;
            _notAllBallsGenerated = false;
            
            while (!_userStepFinished)
                yield return null;
            
            CheckLowEmptySpace();
        }

        var waitForGameFailPanelReady = new PushPopupAndWaitForScreenReady<UIGameFailPanel>(new UIGameFailPanelData() { GameProcessor = this });
        yield return waitForGameFailPanelReady;
        
        var waitForGameFailPanelClosed = new WaitForScreenClosed(waitForGameFailPanelReady.Panel);
        yield return waitForGameFailPanelClosed;

        _field.Clear();
        CheckLowEmptySpace();

        ApplicationController.Instance.UIPanelController.HideAll();
        ApplicationController.Instance.UIPanelController.PushScreen(typeof(UIStartPanel), new UIStartPanelData(){GameProcessor = this}, UIStartPanel_OnScreenReady);
    }

    private void Restart()
    {
      
    }

    private void OnScreenReady(UIPanel obj)
    {
        ApplicationController.Instance.UIPanelController.PushScreen(typeof(UIStartPanel), new UIStartPanelData(){GameProcessor = this}, UIStartPanel_OnScreenReady);
    }

    private void CastleSelector_OnCastleCompleted()
    {
        SelectNextCastle();
    }
    
    void Field_OnPointerDown(Vector3Int pointerGridPosition)
    {
        var balls = _field.GetSomething<Ball>(pointerGridPosition).ToList();
        Ball ball = null;
        if(balls.Count != 0)
            ball = balls[0];
            
        _otherSelectedBall = null;
    
        if (_selectedBall != null)
        {
            if (ball != null)
            {
                if (_selectedBall == ball)
                {
                    _stepMachine.AddStep(new Step(StepTag.Deselect, new SelectOperation(pointerGridPosition, false, _field)
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
                            _stepMachine.AddStep(new Step(StepTag.Merge,
                                new MoveOperation(_selectedBall.IntGridPosition, pointerGridPosition, _field),
                                new MergeOperation(pointerGridPosition, _field),
                                new SelectOperation(pointerGridPosition, false, _field)
                                    .SubscribeCompleted(OnDeselectBall),
                                new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this),
                                new CheckIfGenerationIsNecessary(
                                    null,
                                    new List<Operation>(){
                                        new GenerateOperation(_generatedBallsCountAfterMerge, _generatedBallsCountAfterMove, _generatedBallsPointsRange, _field),
                                        new CollapseOperation(_collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this)})));
                        }
                        else
                        {
                            _stepMachine.AddStep(new Step(StepTag.NoPath,
                                new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition, _noPathEffectPrefab, _field)));
                        }
                    }
                    else
                    {
                        _stepMachine.AddStep(new Step(StepTag.ChangeSelected, 
                            new SelectOperation(_selectedBall.IntGridPosition, false, _field).SubscribeCompleted(OnDeselectBall),
                            new SelectOperation(pointerGridPosition, true, _field).SubscribeCompleted(OnSelectBall)));
                    }
                }
            }
            else
            {
                var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                if (path.Count > 0)
                {
                    _stepMachine.AddStep(new Step(StepTag.Move, 
                        new MoveOperation(_selectedBall.IntGridPosition, pointerGridPosition, _field),
                        new SelectOperation(pointerGridPosition, false, _field)
                            .SubscribeCompleted(OnDeselectBall),
                        new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this),
                        new CheckIfGenerationIsNecessary(
                            null,
                            new List<Operation>(){
                                new GenerateOperation(_generatedBallsCountAfterMove, _generatedBallsCountAfterMove, _generatedBallsPointsRange, _field),
                                new CollapseOperation( _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this)
                            })));
                }
                else
                {
                    _stepMachine.AddStep(new Step(StepTag.NoPath,
                        new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition, _noPathEffectPrefab, _field)));
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
    
    private void OnSelectBall(Operation sender, object result)
    {
        _selectedBall = (Ball)result;
    }
    
    private void OnDeselectBall(Operation sender, object result)
    {
        _selectedBall = null;
    }
    
    private void StepMachine_OnStepExecute(Step step, StepExecutionType executionType)
    {
        OnStepExecute?.Invoke(step, executionType);
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
            if(generateOperationData != null)
                _notAllBallsGenerated = generateOperationData.NewBallsData.Count < generateOperationData.RequiredAmount;
            _userStepFinished = true;
        }
        
        OnStepCompleted?.Invoke(step, executionType);
    }
    
    
    public bool HasUndoSteps()
    {
        return _stepMachine.HasUndoSteps();
    }

    public int MinimalBallsInLine => _minimalBallsInLine;
    public List<Buff> Buffs => _buffs;
    

    public void AddPoints(int points)
    {
        _score += points;
        OnScoreChanged?.Invoke(points);
    }

    public void RemovePoints(int points)
    {
        _score -= points;
        OnScoreChanged?.Invoke(points);
    }
    
    public bool UseUndoBuff(int cost)
    {
        if (!HasUndoSteps()) return false;
        
        _stepMachine.AddStep(
            new Step(StepTag.Undo,
                new SpendOperation(cost, _playerInfo, false),
                new UndoOperation(_stepMachine)));
        return true;
    }

    public void UseExplodeBuff(int cost, ExplodeType explodeType, List<Vector3Int> ballsIndexes)
    {
        _stepMachine.AddStep(
            new Step(ExplodeTypeToStepTags[explodeType], 
                new SpendOperation(cost, _playerInfo, true),
                new RemoveOperation(ballsIndexes, _field)));
    }

    public void UseShowNextBallsBuff(int cost, INextBallsShower nextBallsShower)
    {
        _stepMachine.AddStep(
            new Step(StepTag.NextBalls, 
                new SpendOperation(cost, _playerInfo, true),
                new NextBallsShowOperation(true, nextBallsShower)));
    }
    
    public bool UseDowngradeBuff(int cost, List<Vector3Int> ballsIndexes)
    {
        var gradeLevel = -1;
        var balls = ballsIndexes.SelectMany(i => _field.GetSomething<Ball>(i)).ToList();
        var canGradeAny = balls.Any(ball => ball.CanGrade(gradeLevel));

        if (!canGradeAny)
            return false;

        _stepMachine.AddStep(
            new Step(StepTag.Downgrade,
                new SpendOperation(cost, _playerInfo, true),
                new GradeOperation(ballsIndexes, gradeLevel, _field)));
        return true;
    }

    public void SelectNextCastle()
    {
        foreach (var castle in _castleSelector.Library.Castles)
        {
            var castleProgress = _playerInfo.GetCastleProgress(castle.Name);
            
            if (castleProgress == null || !castleProgress.IsCompleted)
            {
                _playerInfo.SelectCastle(castle.Name);
                StartCoroutine(PlayNewCastleSelectedFx());
                break;
            }
        }
    }
    
    private IEnumerator PlayNewCastleSelectedFx()
    {
        _castleOpenEffect.gameObject.SetActive(true);
        _castleOpenEffect.Play();
        yield return new WaitForSeconds(_castleOpenEffect.main.duration);
    }
}