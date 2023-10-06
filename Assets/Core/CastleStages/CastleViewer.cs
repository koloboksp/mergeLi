
using System.Collections;
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
    
    [SerializeField] private int stageCount;
    [SerializeField] private float flipTime = 1f;
    [SerializeField] private float glowTime = 2f;

    [SerializeField] private AnimationCurve flipCurve;
    [SerializeField] private AnimationCurve glowCurve;

    private int stage;
    private float load;

    // work with mesh renderer
    // [SerializeField] private Renderer rend;
    // private MaterialPropertyBlock mpb;

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
}


