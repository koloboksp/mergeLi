using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
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

public class GameProcessor : MonoBehaviour, IRules, IPointsChangeListener
{
    public const string MoveStepTag = "Move";
    public const string MergeStepTag = "Merge";
    public const string UndoMoveStepTag = "UndoMove";
    public const string UndoMergeStepTag = "UndoMerge";
    public const string ExplodeStepTag = "Explode";
    public const string UndoExplodeStepTag = "UndoExplode";
    public const string UndoStepTag = "Undo";

    public event Action<Step> OnStepCompleted;
    public event Action<Step> OnStepExecute;
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
        _field.OnClick += Field_OnClick;
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
    
    void Field_OnClick(Vector3Int pointerGridPosition)
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
                    _stepMachine.AddStep(new Step("Deselect", new SelectOperation(pointerGridPosition, false, _field)
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
                            _stepMachine.AddStep(new Step(GameProcessor.MergeStepTag,
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
                            _stepMachine.AddStep(new Step("NoPath",
                                new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition, _noPathEffectPrefab, _field)));
                        }
                    }
                }
            }
            else
            {
                var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                if (path.Count > 0)
                {
                    _stepMachine.AddStep(new Step(GameProcessor.MoveStepTag, 
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
                    _stepMachine.AddStep(new Step("NoPath",
                        new PathNotFoundOperation(_selectedBall.IntGridPosition, pointerGridPosition, _noPathEffectPrefab, _field)));
                }
            }
        }
        else
        {
            if (ball != null)
            {
                _stepMachine.AddStep(new Step("Select", new SelectOperation(pointerGridPosition, true, _field)
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
    
    private void StepMachine_OnStepExecute(Step step)
    {
        OnStepExecute?.Invoke(step);
    }

    private void CheckLowEmptySpace()
    {
        var emptyCellsCount = _field.CalculateEmptySpacesCount();
        var threshold = Mathf.Max(_generatedBallsCountAfterMerge, _generatedBallsCountAfterMove);
        bool lowSpace = emptyCellsCount <= threshold;
        
        OnLowEmptySpaceChanged?.Invoke(lowSpace);
    }
    
    private void StepMachine_OnStepCompleted(Step step)
    {
        if (step.Tag == GameProcessor.MergeStepTag)
        {
            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoMergeStepTag, inverseOperations));
            
            var generateOperationData = step.GetData<GenerateOperationData>();
            if(generateOperationData != null)
                _notAllBallsGenerated = generateOperationData.NewBallsData.Count < generateOperationData.RequiredAmount;
            _userStepFinished = true;
        }
            
        if (step.Tag == GameProcessor.MoveStepTag)
        {
            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoMoveStepTag, inverseOperations));
           
            var generateOperationData = step.GetData<GenerateOperationData>();
            if(generateOperationData != null)
                _notAllBallsGenerated = generateOperationData.NewBallsData.Count < generateOperationData.RequiredAmount;
            _userStepFinished = true;
        }
            
        if (step.Tag == GameProcessor.ExplodeStepTag)
        {
            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoExplodeStepTag, inverseOperations));
            
            _userStepFinished = true;
        }
        OnStepCompleted?.Invoke(step);
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
    
    public void UseUndoBuff(int cost)
    { 
        _stepMachine.AddStep(
            new Step(GameProcessor.UndoStepTag,
                new SpendOperation(cost, _playerInfo, false),
                new UndoOperation(_stepMachine)));
    }

    public void UseExplodeBuff(int cost, List<Vector3Int> ballsIndexes)
    {
        _stepMachine.AddStep(
            new Step(GameProcessor.ExplodeStepTag, 
                new SpendOperation(cost, _playerInfo, true),
                new RemoveOperation(ballsIndexes, _field)));
    }

    public void UseShowNextBallsBuff(int cost)
    {
        
    }

    public void SelectNextCastle()
    {
        foreach (var castle in _castleSelector.Library.Castles)
        {
            var castleProgress = _playerInfo.GetCastleProgress(castle.Name);
            
            if (castleProgress == null || !castleProgress.IsCompleted)
            {
                _playerInfo.SelectCastle(castle.Name);
                break;
            }
        }
        
    }
}