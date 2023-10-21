
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
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

        _cancellationTokenSource = new CancellationTokenSource();
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

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 200, 100), "Reset Castle"))
            SetStage(1);

        if (GUI.Button(new Rect(20, 120, 200, 100), "Add Coins"))
            AddPoints(.25f);

    }
    
    private readonly List<Operation> _operations = new ();
    private CancellationTokenSource _cancellationTokenSource;
    
    public void ShowPartBorn(int partIndex, bool instant)
    {
        var operation = new ShowBornProgressOperation(partIndex, 0, 1, this);
        if (instant)
            operation.ExecuteInstant();
        else
            _operations.Add(operation);
    }
    
    public void ShowPartDeath(int partIndex, bool instant)
    {
        var operation = new ShowBornProgressOperation(partIndex, 1, 0, this);
        if (instant)
            operation.ExecuteInstant();
        else
            _operations.Add(operation);
    }
    
    public void ShowPartProgress(int partIndex, float nOldPoints, float nNewPoints, bool instant)
    {
        var operation = new ShowPartProgressOperation(partIndex, nOldPoints, nNewPoints, this);
        if (instant)
            operation.ExecuteInstant();
        else
            _operations.Add(operation);
    }
    
    public void ShowPartComplete(int partIndex, bool instant)
    {
        var operation = new ShowPartCompleteOperation(partIndex, this);
        if (instant)
            operation.ExecuteInstant();
        else
            _operations.Add(operation);
    }
    
    
    private async void Start()
    {
        await WaitForOperations(_cancellationTokenSource.Token);
    }

    private async Task WaitForOperations(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (_operations.Count > 0)
                {
                    var operation = _operations[0];
                    
                    await operation.ExecuteAsync(cancellationToken);
                    
                    _operations.RemoveAt(0);
                }
                else
                {
                    await Task.Yield();
                }
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
    }
    
    public class Operation
    {
        private int _partIndex;
        private CastleViewer _target; 

        public int PartIndex => _partIndex;
        public CastleViewer Target => _target;

        protected Operation(int partIndex, CastleViewer target)
        {
            _partIndex = partIndex;
            _target = target;
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken) { }
        
        public virtual void ExecuteInstant() { }
        
        protected async Task ChangeValueOperationAsync(AnimationCurve curve, float dur, int prop, float v0, float v1, CancellationToken cancellationToken)
        {
            float timer = dur;
            float scale = v1 - v0;

            while (timer > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            
                timer -= Time.deltaTime;
                if (timer < 0)
                    timer = 0;

                _target.mat.SetFloat(prop, v0 + curve.Evaluate(1f - timer / dur) * scale);
                // rend.SetPropertyBlock(mpb);

                await Task.Yield();
            }
        }
        
        protected void ChangeValueOperationInstant(AnimationCurve curve, float dur, int prop, float v0, float v1)
        {
            _target.mat.SetFloat(prop, v1);
        }

    }
    
    public class ShowPartCompleteOperation : Operation
    {
        public ShowPartCompleteOperation(int partIndex, CastleViewer target) 
            : base(partIndex, target)
        {
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE,  Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);
            
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.BAR_OVER, 0, 1, cancellationToken);
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.GLOW, 0, 1, cancellationToken);
            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.GLOW, 1, 0, cancellationToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE,  Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);
            
            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_OVER, 0, 1);
        }
    }
    
    public class ShowPartProgressOperation : Operation
    {
        private readonly float _nOldPoints; 
        private readonly float _nNewPoints; 
        public ShowPartProgressOperation(int partIndex, float nOldPoints, float nNewPoints, CastleViewer target) 
            : base(partIndex, target)
        {
            _nOldPoints = nOldPoints;
            _nNewPoints = nNewPoints;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_BORN, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.BAR_LOAD, _nOldPoints, _nNewPoints, cancellationToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_BORN, 1);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_LOAD, _nOldPoints, _nNewPoints);
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

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 0);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            await ChangeValueOperationAsync(Target.flipCurve, Target.flipTime, Target.BAR_BORN, _nStartValue, _nEndValue, cancellationToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.stage = PartIndex + 1;
            Target.mat.SetInt(Target.STAGE, Target.stage);
            Target.mat.SetFloat(Target.BAR_LOAD, 0);
            Target.mat.SetFloat(Target.BAR_OVER, 0);

            ChangeValueOperationInstant(Target.flipCurve, Target.flipTime, Target.BAR_BORN, _nStartValue, _nEndValue);
        }
    }
}


