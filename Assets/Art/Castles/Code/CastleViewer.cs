
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Effects;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CastleViewer : MonoBehaviour
{
    private const int STAGE_MAX = 31;
    private readonly int STAGE = Shader.PropertyToID("_Stage");

    private readonly int BAR_BORN = Shader.PropertyToID("_BarBorn");
    private readonly int BAR_LOAD = Shader.PropertyToID("_BarLoad");
    private readonly int BAR_OVER = Shader.PropertyToID("_BarOver");

    private readonly int GLOW = Shader.PropertyToID("_Glow");
    private readonly int GRAY = Shader.PropertyToID("_Gray");

    // work with image
    [SerializeField] private Image image;
    private Material mat;

    [SerializeField] private CastleSettings castleSettings;
    [SerializeField] private int stageCount;
    [SerializeField] private RectTransform _root;

    private float flipTime = 1f;
    private float glowTime = 2f;
    private AnimationCurve flipCurve;
    private AnimationCurve glowCurve;

    private int stage;
    private float load;

    // work with mesh renderer
    // [SerializeField] private Renderer rend;
    // private MaterialPropertyBlock mpb;

    [SerializeField] private Transform[] stagePoints;
    public Transform StagePoint(int id) => stagePoints[id];
    public RectTransform Root => _root;
    private void Awake()
    {
        // if (rend == null)
        //     return;
        // 
        // mpb = new MaterialPropertyBlock();

        if (image == null)
            return;

        mat = new Material(image.material);
        image.material = mat;

        SetAll(0, 0, 0, 0, 0);
        // rend.SetPropertyBlock(mpb);

        flipTime = castleSettings.flipTime;
        glowTime = castleSettings.glowTime;
        flipCurve = castleSettings.flipCurve;
        glowCurve = castleSettings.glowCurve;

        _destroyTokenSource = new CancellationTokenSource();
    }

    private void SetAll(float barBorn, float barLoad, float barOver, float glow, float gray)
    {
        SetBars(barBorn, barLoad, barOver);

        mat.SetFloat(GLOW, glow);
        mat.SetFloat(GRAY, gray);
    }

    private void SetBars(float barBorn, float barLoad, float barOver)
    {
        mat.SetFloat(BAR_BORN, barBorn);
        mat.SetFloat(BAR_LOAD, barLoad);
        mat.SetFloat(BAR_OVER, barOver);
    }

    public void SetStage(int stage)
    {
        this.stage = stage;
        load = 0;
        
        mat.SetInt(STAGE, stage);
        SetAll(0, 0, 0, 0, 0);

        StartCoroutine(PlayRoutine(flipCurve, flipTime, BAR_BORN));
    }

    /// <summary>
    /// Points is normalized value
    /// </summary>
    public void AddPoints(float points) => StartCoroutine(MainRoutine(points));

    private IEnumerator MainRoutine(float points)
    {
        float loadOld = load;
        load += points;

        if (load > 1)
            load = 1f;

        // play add points to progress bar
        yield return StartCoroutine(PlayRoutine(flipCurve, flipTime, BAR_LOAD, loadOld, load));

        if (load < 1)
            yield break;

        // play bar over
        yield return PlayRoutine(flipCurve, flipTime, BAR_OVER);

        // play local glow
        yield return PlayRoutine(glowCurve, glowTime, GLOW);

        // reset sliders
        load = 0;
        SetBars(0, 0, 0);

        stage = stage == stageCount ? STAGE_MAX : stage + 1;
        mat.SetFloat(STAGE, stage);
        
        if (stage == STAGE_MAX)
        {
            yield return PlayRoutine(glowCurve, glowTime, GLOW);
        }
        else
        {
            yield return PlayRoutine(flipCurve, flipTime, BAR_BORN);
        }
    }

    private IEnumerator PlayRoutine(AnimationCurve curve, float dur, int prop, float v0 = 0, float v1 = 1f)
    {
        float timer = dur;
        float scale = v1 - v0;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;

            mat.SetFloat(prop, v0 + curve.Evaluate(1f - timer / dur) * scale);
            // rend.SetPropertyBlock(mpb);

            yield return new WaitForEndOfFrame();
        }
    }

    public void MoveToGray() => StartCoroutine(PlayRoutine(flipCurve, flipTime, GRAY));

    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(20, 20, 200, 100), "Reset Castle"))
    //         SetStage(1);
    // 
    //     if (GUI.Button(new Rect(20, 120, 200, 100), "Add Coins"))
    //         AddPoints(.25f);
    // 
    // }

    public event Action<bool, float> OnPartCompleteStart;
    public event Action<bool, float, int, int, int> OnPartProgressStart;
    public event Action<bool, float> OnPartBornStart;
    
    [SerializeField] private CastlePartCompleteEffect _partCompleteEffect;
    
    private readonly List<Operation> _operations = new ();
    private Task _operationsExecutor;
    private CancellationTokenSource _destroyTokenSource;
    
    private void OnDestroy()
    {
        _destroyTokenSource.Cancel();
        _destroyTokenSource.Dispose();
    }

    public void ShowPartBorn(int partIndex, bool instant)
    {
        var operation = new ShowBornProgressOperation(partIndex, 0, 1, this);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    public void ShowPartDeath(int partIndex, bool instant)
    {
        var operation = new ShowBornProgressOperation(partIndex, 1, 0, this);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    public void ShowPartProgress(int partIndex, int oldPoints, int newPoints, int maxPoints, bool instant)
    {
        var operation = new ShowPartProgressOperation(partIndex, oldPoints, newPoints, maxPoints, this);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    public void ShowPartComplete(int partIndex, bool instant)
    {
        var operation = new ShowPartCompleteOperation(partIndex, this, _partCompleteEffect);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    private void AddOperation(Operation operation)
    {
        _operations.Add(operation);

        if (_operationsExecutor == null)
            _operationsExecutor = WaitForOperationsSafe(_destroyTokenSource.Token, Application.exitCancellationToken);
    }
    
    private async Task WaitForOperationsSafe(CancellationToken destroyToken, CancellationToken exitToken)
    {
        try
        {
            while (_operations.Count > 0)
            {
                destroyToken.ThrowIfCancellationRequested();
                exitToken.ThrowIfCancellationRequested();
                
                var operation = _operations[0];

                await operation.ExecuteAsync(destroyToken, exitToken);

                _operations.RemoveAt(0);
            }
        }
        catch (OperationCanceledException e)
        {
            Debug.Log(e);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _operationsExecutor = null;
        }
    }
    
    public class Operation
    {
        private readonly int _partIndex;
        private readonly CastleViewer _target; 

        public int PartIndex => _partIndex;
        public CastleViewer Target => _target;

        protected Operation(int partIndex, CastleViewer target)
        {
            _partIndex = partIndex;
            _target = target;
        }

        public virtual async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken) { }
        
        public virtual void ExecuteInstant() { }
        
        protected async Task ChangeValueOperationAsync(
            AnimationCurve curve, 
            float duration,
            int prop,
            float v0,
            float v1,
            CancellationToken destroyToken,
            CancellationToken exitToken)
        {
            float timer = duration;
            float scale = v1 - v0;

            while (timer > 0)
            {
                destroyToken.ThrowIfCancellationRequested();
                exitToken.ThrowIfCancellationRequested();
            
                timer -= Time.deltaTime;
                if (timer < 0)
                    timer = 0;

                _target.mat.SetFloat(prop, v0 + curve.Evaluate(1f - timer / duration) * scale);
                // rend.SetPropertyBlock(mpb);

                await Task.Yield();
            }
        }
        
        protected void ChangeValueOperationInstant(AnimationCurve curve, float dur, int prop, float v0, float v1)
        {
            _target.mat.SetFloat(prop, v1);
        }
        
        protected async Task PlayEffect(
            CastlePartCompleteEffect completeEffect,
            Transform target,
            CancellationToken destroyToken,
            CancellationToken exitToken)
        {
            var findObjectOfType = FindObjectOfType<UIFxLayer>();
            var effect = Instantiate(completeEffect, findObjectOfType.transform);
            effect.transform.position = target.position;
            effect.Run();
            
            await AsyncExtensions.WaitForSecondsAsync(effect.Duration, destroyToken, exitToken);
        }
    }
    
    public class ShowPartCompleteOperation : Operation
    {
        private readonly CastlePartCompleteEffect _completeEffect;
        
        private DependencyHolder<SoundsPlayer> _soundPlayer;
        public ShowPartCompleteOperation(
            int partIndex, 
            CastleViewer target, 
            CastlePartCompleteEffect completeEffect) 
            : base(partIndex, target)
        {
           
            _completeEffect = completeEffect;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            float duration = Target.flipTime + Target.flipTime + Target.flipTime;
            Target.OnPartCompleteStart?.Invoke(false, duration);
            
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE,  Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);
            
            await PlayEffect(_completeEffect, this.Target.stagePoints[PartIndex], destroyToken, exitToken);
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.BAR_OVER, 0, 1, destroyToken, exitToken);
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.GLOW, 0, 1, destroyToken, exitToken);
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.GLOW, 1, 0, destroyToken, exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.OnPartCompleteStart?.Invoke(true, 0.0f);
            
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE,  Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);
            
            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_OVER, 0, 1);
        }
    }
    
    public class ShowPartProgressOperation : Operation
    {
        private readonly int _oldPoints; 
        private readonly int _newPoints;
        private readonly int _maxPoints;
        public ShowPartProgressOperation(int partIndex, int oldPoints, int newPoints, int maxPoints, CastleViewer target) 
            : base(partIndex, target)
        {
            _oldPoints = oldPoints;
            _newPoints = newPoints;
            _maxPoints = maxPoints;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = Target.flipTime;
            Target.OnPartProgressStart?.Invoke(false, duration, _oldPoints, _newPoints, _maxPoints);

            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_BORN, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            var nOldPoints = (float)_oldPoints / _maxPoints;
            var nNewPoints = (float)_newPoints / _maxPoints;
            
            await ChangeValueOperationAsync(
                Target.flipCurve, 
                Target.flipTime, 
                Target.BAR_LOAD, 
                nOldPoints, 
                nNewPoints, 
                destroyToken,
                exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.OnPartProgressStart?.Invoke(true, 0.0f, _oldPoints, _newPoints, _maxPoints);

            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_BORN, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);
           
            var nOldPoints = (float)_oldPoints / _maxPoints;
            var nNewPoints = (float)_newPoints / _maxPoints;

            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_LOAD, nOldPoints, nNewPoints);
        }
    }
    
    
    public class ShowBornProgressOperation : Operation
    {
        private readonly float _nStartValue; 
        private readonly float _nEndValue; 
        public ShowBornProgressOperation(int partIndex, float nStartValue, float nEndValue, CastleViewer target) 
            : base(partIndex, target)
        {
            _nStartValue = nStartValue;
            _nEndValue = nEndValue;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = Target.flipTime;
            Target.OnPartBornStart?.Invoke(false, duration);

            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 0);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            await ChangeValueOperationAsync(
                Target.flipCurve,
                Target.flipTime, 
                Target.BAR_BORN, 
                _nStartValue, 
                _nEndValue, 
                destroyToken,
                exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.OnPartBornStart?.Invoke(true, 0.0f);

            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 0);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_BORN, _nStartValue, _nEndValue);
        }
    }
}


