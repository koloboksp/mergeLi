
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Effects;
using UnityEngine;

public class CastleBit : MonoBehaviour
{
    private const float BORN_TIME = .5f;
    private const float GROW_TIME = .5f;
    private const float FULL_TIME = 1f;
    private const int ONE_BIT_DELAY = 50; // in ms

    private static readonly int pAlpha = Shader.PropertyToID("_Alpha");
    private static readonly int pLevel = Shader.PropertyToID("_Level");
    private static readonly int pGlow = Shader.PropertyToID("_Glow");
    private static readonly int pGray = Shader.PropertyToID("_Gray");
    private static readonly int pRing = Shader.PropertyToID("_Ring");
    private static readonly int pScale = Shader.PropertyToID("_Scale");
    private static readonly int pBorder = Shader.PropertyToID("_Border");

    public enum ShowMode { Born, Death, Grow, FullBit, FullCastle }

    private RectTransform rect;
    private RenderTexture rTex;
    private Material mat;

    public RectTransform Rect
    {
        get { return rect; }
    }

    // Scores are normalized
    public float price;
    public float priceMin;
    public float priceMax;

    // To play bits
    private float curProgress = -1f;
    private float newProgress = -2f;

    public void Init(RectTransform rect, RenderTexture rTex, Material mat)
    {
        this.rect = rect;
        this.rTex = rTex;
        this.mat = mat;

        SetMaterialProps(0, 0, 0, 1, 0, 0, 1);
    }

    public void Destroy()
    {
        if (rTex != null && rTex.IsCreated())
            rTex.Release();
    }

    public async Task SetScore(float newValue)
    {
        newProgress = (newValue - priceMin) / price;

        if (newProgress < 0 && curProgress < 0 ||
            newProgress > 1 && curProgress > 1 ||
            newProgress == curProgress)
            return;

        if (curProgress < 0)
        {
            await Play(ShowMode.Born, BORN_TIME);
            curProgress = 0;
        }

        if (curProgress >= 0 && newProgress > 0)
            await Play(ShowMode.Grow, GROW_TIME);
        
        if (newProgress > 1)
            await Play(ShowMode.FullBit, FULL_TIME);

        curProgress = newProgress;
    }

    public void ResetProgress()
    {
        SetMaterialPhase(ShowMode.Death, 1);
    }
    public async Task PlayComplete(int delayIndex)
    {
        await Task.Delay(delayIndex * ONE_BIT_DELAY);

        await Play(ShowMode.FullCastle, FULL_TIME);
    }

    private async Task Play(ShowMode showMode, float time)
    {
        float timer = time;

        while (timer > 0)
        {
            timer = timer > 0 ? timer - Time.deltaTime : 0;

            SetMaterialPhase(showMode, 1f - timer / time);

            await Task.Yield();
        }
    }

    private void SetMaterialPhase(ShowMode playMode, float phase)
    {
        float softPhase = Mathf.SmoothStep(0, 1f, phase);
        float softHill = 1f - Mathf.Abs(softPhase * 2f - 1f);

        switch (playMode)
        {
            case ShowMode.Born:
                SetMaterialProps(softPhase, 0, 0, 1f, 0, 1f - softPhase, 1f);
                break;
            
            case ShowMode.Death:
                SetMaterialProps(1f - softPhase, 0, 0, 1f, 0, 0 , 1f);
                break;
            
            case ShowMode.Grow:
                SetMaterialProps(1f, Mathf.Lerp(curProgress, newProgress, softPhase), phase, 1f, 0f, 0, 1f);
                break;

            case ShowMode.FullBit:
                SetMaterialProps(1f, 0f, 0, 0f, 1f - Mathf.Clamp01(phase * phase * 4f), softHill, 1f - softPhase);
                break;

            case ShowMode.FullCastle:
                SetMaterialProps(1f, 0, phase, 0, 0, softHill, 0);
                break;
        }
    }

    private void SetMaterialProps(float alpha, float level, float glow, float gray, float ring, float scale, float border)
    {
        mat.SetFloat(pAlpha, alpha);
        mat.SetFloat(pGray, gray);

        mat.SetFloat(pLevel, level);
        mat.SetFloat(pRing, ring);

        mat.SetFloat(pGlow, glow);
        mat.SetFloat(pScale, scale);
        mat.SetFloat(pBorder, border);
    }
    
    public class ShowBornProgressOperation : CastleViewer2.Operation
    {
        private readonly int _maxPoints;
        public ShowBornProgressOperation(int partIndex, int maxPoints, CastleViewer2 target, CastleBit bit) 
            : base(partIndex, target, bit)
        {
            _maxPoints = maxPoints;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = BORN_TIME;
            Target.CallOnPartBornStart(false, duration, _maxPoints);
            
            await Bit.ChangeValueOperationAsync(Bit, ShowMode.Born, duration, destroyToken, exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.CallOnPartBornStart(false, 0.0f, _maxPoints);
            
            Bit.ChangeValueOperationInstant(Bit, ShowMode.Born, 1);
        }
    }
    
    public class ShowDeathProgressOperation : CastleViewer2.Operation
    {
        private readonly int _maxPoints;
        public ShowDeathProgressOperation(int partIndex, int maxPoints, CastleViewer2 target, CastleBit bit) 
            : base(partIndex, target, bit)
        {
            _maxPoints = maxPoints;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = BORN_TIME;
            Target.CallOnPartBornStart(false, duration, _maxPoints);
            
            await Bit.ChangeValueOperationAsync(Bit, ShowMode.Death, duration, destroyToken, exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.CallOnPartBornStart(false, 0.0f, _maxPoints);
            
            Bit.ChangeValueOperationInstant(Bit, ShowMode.Death, 1);
        }
    }
    
    public class ShowPartProgressOperation : CastleViewer2.Operation
    {
        private readonly int _oldPoints; 
        private readonly int _newPoints;
        private readonly int _maxPoints;
        public ShowPartProgressOperation(int partIndex, int oldPoints, int newPoints, int maxPoints, CastleViewer2 target, CastleBit bit) 
            : base(partIndex, target, bit)
        {
            _oldPoints = oldPoints;
            _newPoints = newPoints;
            _maxPoints = maxPoints;
        }
        
        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = GROW_TIME;
            Target.CallOnPartProgressStart(false, duration, _oldPoints, _newPoints, _maxPoints);

            Bit.newProgress = (float)_newPoints / _maxPoints;
            Bit.curProgress = (float)_oldPoints / _maxPoints;
            await Bit.ChangeValueOperationAsync(Bit, ShowMode.Grow, duration, destroyToken, exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.CallOnPartProgressStart(true, 0.0f, _oldPoints, _newPoints, _maxPoints);
            
            var nOldPoints = (float)_oldPoints / _maxPoints;
            var nNewPoints = (float)_newPoints / _maxPoints;
            Bit.newProgress = nNewPoints;
            Bit.curProgress = nOldPoints;
            Bit.ChangeValueOperationInstant(Bit, ShowMode.Grow, 1.0f);
        }
    }
    
    public class ShowPartCompleteOperation : CastleViewer2.Operation
    {
        private readonly CastlePartCompleteEffect _completeEffect;
        
        private DependencyHolder<SoundsPlayer> _soundPlayer;
        public ShowPartCompleteOperation(
            int partIndex, 
            CastleViewer2 target, 
            CastlePartCompleteEffect completeEffect, 
            CastleBit bit) 
            : base(partIndex, target, bit)
        {
           
            _completeEffect = completeEffect;
        }

        public override async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken)
        {
            var duration = FULL_TIME;
            Target.CallOnPartCompleteStart(false, duration);
            
            await PlayEffect(_completeEffect, Bit.Rect, destroyToken, exitToken);
            await Bit.ChangeValueOperationAsync(Bit, ShowMode.FullBit, duration, destroyToken, exitToken);
        }
        
        public override void ExecuteInstant()
        {
            Target.CallOnPartCompleteStart(false, 0.0f);
            
            Bit.ChangeValueOperationInstant(Bit, ShowMode.FullBit, 1.0f);
        }
    }
    
    protected async Task ChangeValueOperationAsync(
        CastleBit bit, 
        ShowMode showMode,
        float duration,
        CancellationToken destroyToken,
        CancellationToken exitToken)
    {
        float timer = duration;
         
        while (timer > 0)
        {
            destroyToken.ThrowIfCancellationRequested();
            exitToken.ThrowIfCancellationRequested();
            
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;
            
            bit.SetMaterialPhase(showMode, 1.0f - Mathf.Clamp01(timer/duration));

            await Task.Yield();
        }
    }
        
    protected void ChangeValueOperationInstant(
        CastleBit bit, 
        ShowMode showMode,
        float value)
    {
        bit.SetMaterialPhase(showMode, value);
    }
}
