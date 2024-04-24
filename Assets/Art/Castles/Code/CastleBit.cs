
using System.Threading.Tasks;
using UnityEngine;

public class CastleBit
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

    private enum ShowMode { Born, Grow, FullBit, FullCastle }

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

    public CastleBit(RectTransform rect, RenderTexture rTex, Material mat)
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
}
