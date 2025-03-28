using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Effects;
using Core.Steps.CustomOperations;
using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

public class CastleViewer2 : MonoBehaviour
{
    private const float BACK_SCALE = .25f;
    private const int BACK_BLUR_COUNT = 2;

    private const float MASK_SCALE = .5f; // Relative to main image
    private const int PIX_PADDING = 4;

    [SerializeField] private Image image;
    [SerializeField] private ImagePattern pattern;
    [SerializeField] private CastleViewerPreset preset;

    [SerializeField] private int maxScore = 1000;
    [SerializeField] private int nowScore = 0;
    [SerializeField] private RectTransform _root;

    private CastleBit[] bits;
    private RenderTexture rTexBack;
    
    public RectTransform Root => _root;
    
    // Debug
    private readonly int[] testScore = new int[] { 100, 500, 1000, -100, -500, -1000 };

    private void Awake()
    {
        _destroyTokenSource = new CancellationTokenSource();
        
        MakeCastleBits();
        CalcPrices();

        image.enabled = false;

        //AddScore(0);
    }

    private void MakeCastleBits()
    {
        var tempCameraObject = new GameObject("MakeBitsCamera");
        var tempCamera = tempCameraObject.AddComponent<Camera>();
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = Color.black;
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = 1;
        tempCamera.enabled = false;
       
        bits = new CastleBit[pattern.bits.Count];

        RectTransform baseRectTrans = GetComponent<RectTransform>();
        var maskPixToBaseRect = new Vector2(
               1f / MASK_SCALE * baseRectTrans.sizeDelta.x / pattern.image.width,
               1f / MASK_SCALE * baseRectTrans.sizeDelta.y / pattern.image.height);
        
        var matVertColor = new Material(preset.shVertColor);
        var matMultAlpha = new Material(preset.shMultAlpha);
        var matBlur = new Material(preset.shBlur);


        // Make the blured texture for background effect
        int bw = (int)(pattern.image.width * BACK_SCALE);
        int bh = (int)(pattern.image.height * BACK_SCALE);
        rTexBack = new RenderTexture(bw, bh, 0, RenderTextureFormat.ARGB32, 0);
        rTexBack.Create();
        var rTexBackTemp = RenderTexture.GetTemporary(bw, bh, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(pattern.image, rTexBack);
        for (int i = 0; i < BACK_BLUR_COUNT; i++)
        {
            Graphics.Blit(rTexBack, rTexBackTemp, matBlur);
            Graphics.Blit(rTexBackTemp, rTexBack, matBlur);
        }
        RenderTexture.ReleaseTemporary(rTexBackTemp);

        int w = (int)(pattern.image.width * MASK_SCALE);
        int h = (int)(pattern.image.height * MASK_SCALE);

        var verts = ImagePatternSolver.LoadPattern(pattern);
        Rect rect; // In normal space
        
        for (int i = 0; i < verts.Count; i++)
        {
            // Looking for bit rect in OneSpace
            rect = new Rect(verts[i][0], Vector2.zero);
            for (int j = 1; j < verts[i].Count; j++)
            {
                rect.min = Vector2.Min(rect.min, verts[i][j]);
                rect.max = Vector2.Max(rect.max, verts[i][j]);
            }
            
            // Clamp to [0..1]
            rect.min = Vector2.Max(rect.min, Vector2.zero);
            rect.max = Vector2.Min(rect.max, Vector2.one);

            // Convert to Pixels & Apply Padding
            int px = (int)(rect.xMin * w) - PIX_PADDING;
            int py = (int)(rect.yMin * h) - PIX_PADDING;
            int pw = (int)(rect.width * w) + PIX_PADDING * 2;
            int ph = (int)(rect.height * h) + PIX_PADDING * 2;

            // Render Mesh to RenderTexture
            var tris = ImagePatternSolver.PolyToTris(verts[i].ToArray());
            var rTexTemp = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.R8);
            
            tempCamera.targetTexture = rTexTemp;
            var previousCurrentCamera = Camera.current;
            Camera.SetupCurrent(tempCamera);
            
            var tmpMesh = new Mesh();
            var vertices = new Vector3[tris.Length];
            var indices = new int[tris.Length];

            for (var j = 0; j < tris.Length; j++)
            {
                var v = verts[i][tris[j]];
                v.y = 1.0f - v.y;
                v -= new Vector2(0.5f, 0.5f);
                v *= 2.0f;
                v.x *= (float)w / (float)h;
                vertices[j] = v;
                indices[j] = j;
            }

            tmpMesh.SetVertices(vertices);
            tmpMesh.SetIndices(indices, MeshTopology.Triangles, 0);
            tmpMesh.UploadMeshData(false);
            
            tempCamera.Render();
            preset.matCastleBit.SetPass(0);
            Graphics.DrawMeshNow(tmpMesh, new Vector3(0, 0, 1), Quaternion.identity);
           
            Destroy(tmpMesh);
            
            tempCamera.targetTexture = null;
            Camera.SetupCurrent(previousCurrentCamera);
            
            // Mult with the main image of castle to use its alpha
            Graphics.Blit(pattern.image, rTexTemp, matMultAlpha);

            // Copy part of rTexTemp which is inside the Rect
            var rTex = new RenderTexture(pw, ph, 0, RenderTextureFormat.R8, 0);
            rTex.Create();
            var scale = new Vector2(pw / (float)w, ph / (float)h);
            var offset = new Vector2(px / (float)w, 1f - (py + ph) / (float)h);
            Graphics.Blit(rTexTemp, rTex, scale, offset);
            RenderTexture.ReleaseTemporary(rTexTemp);

            // Blur it a little time
            var rTexBlurTmp = RenderTexture.GetTemporary(pw, ph, 0, RenderTextureFormat.R8);
            Graphics.Blit(rTex, rTexBlurTmp, matBlur);
            Graphics.Blit(rTexBlurTmp, rTex, matBlur);
            RenderTexture.ReleaseTemporary(rTexBlurTmp);
                
            // Make an object with this texture holder
            var newObj = new GameObject("Bit_" + i);
            var trans = newObj.transform;
            trans.parent = transform;
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;

            var rTrans = newObj.AddComponent<RectTransform>();
            rTrans.anchorMin = Vector2.up; // top left corner of base RectTransform 
            rTrans.anchorMax = Vector2.up;
            rTrans.sizeDelta = Vector2.Scale(new Vector2(pw, ph), maskPixToBaseRect);
            rTrans.anchoredPosition = Vector2.Scale(new Vector2(px + pw / 2f, -py - ph / 2f), maskPixToBaseRect);

            var matBit = new Material(preset.matCastleBit) { name = newObj.name };
            matBit.SetTexture("_MainTex", pattern.image);
            matBit.SetVector("_MainTex_ST", new Vector4(scale.x, scale.y, offset.x, offset.y)); ;
            matBit.SetTexture("_Mask", rTex);
            matBit.SetVector("_Pivot", rTrans.position);
            matBit.SetTexture("_Back", rTexBack);

            var img = newObj.AddComponent<Image>();
            img.material = matBit;

            var castleBit = newObj.AddComponent<CastleBit>();
            castleBit.Init(rTrans, rTex, matBit);
            
            bits[i] = castleBit;
        }

        // Set Reversed bits order
        foreach (var bit in bits)
            bit.Rect.SetAsFirstSibling();
        
        Destroy(tempCameraObject);
    }

    private void CalcPrices()
    {
        // Calc Prices propotional to bit square
        var sum = 0f;
        for (int i = 0; i < bits.Length; i++)
        {
            bits[i].price = bits[i].Rect.sizeDelta.x * bits[i].Rect.sizeDelta.y;
            sum += bits[i].price; 
        }

        float full = 0;
        for (int i = 0; i < bits.Length; i++)
        {
            bits[i].price /= sum; // normalized price for this bit
            bits[i].priceMin = full; // minimum price when this bit is empty
            full += bits[i].price;
            bits[i].priceMax = full; // maximum price when this bit is full
        }
    }

    //private void OnDestroy()
    //{
    //    for (int i = 0; i < bits.Length; i++)
    //        bits[i].Destroy();
    //}

    public async void AddScore(int coins)
    {
        nowScore += coins;

        if (nowScore < maxScore)
        {
            foreach (var bit in bits)
                await bit.SetScore(nowScore / (float)maxScore);
        }
        
        if (nowScore > maxScore)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < bits.Length; i++)
                tasks.Add(bits[i].PlayComplete(i));

            await Task.WhenAll(tasks);
        }
    }
    
    // For Debug
    // private void OnGUI()
    // {
    //     GUI.BeginGroup(new Rect(20, 20, 200, 500));
    //
    //     for (int i = 0; i < testScore.Length; i++)
    //         if (GUILayout.Button("Add " + testScore[i]))
    //             AddScore(testScore[i]);
    //
    //     GUI.EndGroup();
    // }
    //
    public event Action<bool, float> OnPartCompleteStart;
    public event Action<bool, float, int, int, int> OnPartProgressStart;
    public event Action<bool, float, int> OnPartBornStart;

    internal void CallOnPartProgressStart(bool instant, float duration, int oldPoints, int newPoints, int maxPoints)
    {
        OnPartProgressStart?.Invoke(instant, duration, oldPoints, newPoints, maxPoints);
    }
    internal void CallOnPartBornStart(bool instant, float duration, int maxPoints)
    {
        OnPartBornStart?.Invoke(instant, duration, maxPoints);
    }
    internal void CallOnPartCompleteStart(bool instant, float duration)
    {
        OnPartCompleteStart?.Invoke(instant, duration);
    }
    
    [SerializeField] private CastlePartCompleteEffect _partCompleteEffect;
    
    private readonly List<Operation> _operations = new ();
    private Task _operationsExecutor;
    private CancellationTokenSource _destroyTokenSource;
    private TaskCompletionSource<bool> _animationsCompletedSource;

    private void OnDestroy()
    {
        _destroyTokenSource.Cancel();
        _destroyTokenSource.Dispose();
        
        for (int i = 0; i < bits.Length; i++)
            bits[i].Destroy();
    }

    public void ResetProgress()
    {
        foreach (var castleBit in bits)
            castleBit.ResetProgress();
    }
    
    public void ShowPartBorn(int partIndex, int maxPoints, bool instant)
    {
        var operation = new CastleBit.ShowBornProgressOperation(partIndex, maxPoints, this, bits[partIndex]);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    public void ShowPartDeath(int partIndex, int maxPoints, bool instant)
    {
        var operation = new CastleBit.ShowDeathProgressOperation(partIndex, maxPoints, this, bits[partIndex]);
        if (instant)
            operation.ExecuteInstant();
        else
            AddOperation(operation);
    }
    
    public void ShowPartProgress(int partIndex, int oldPoints, int newPoints, int maxPoints, bool instant)
    {
        var operation = new CastleBit.ShowPartProgressOperation(partIndex, oldPoints, newPoints, maxPoints, this, bits[partIndex]);
        if (instant)
            operation.ExecuteInstant();
        else
        {
            AddOperation(operation);
        }
    }
    
    public void ShowPartComplete(int partIndex, bool instant)
    {
        var operation = new CastleBit.ShowPartCompleteOperation(partIndex, this, _partCompleteEffect, bits[partIndex]);
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

                if (_operations.Count == 0 && _animationsCompletedSource != null)
                    _animationsCompletedSource.TrySetResult(true);
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
        private readonly CastleViewer2 _target; 
        private readonly CastleBit _bit; 

        public int PartIndex => _partIndex;
        public CastleViewer2 Target => _target;
        public CastleBit Bit => _bit;

        protected Operation(int partIndex, CastleViewer2 target, CastleBit bit)
        {
            _partIndex = partIndex;
            _bit = bit;
            _target = target;
        }

        public virtual async Task ExecuteAsync(CancellationToken destroyToken, CancellationToken exitToken) { }
        
        public virtual void ExecuteInstant() { }
        
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
    
    public async Task WaitForAnimationsComplete(CancellationToken exitToken)
    {
        if (_operations.Count == 0)
            return;
        
        var exitTokenCompletion = new TaskCompletionSource<bool>();
        var exitTokenRegistration = exitToken.Register(() => exitTokenCompletion.TrySetCanceled(exitToken));
        
        try
        {
            if (_animationsCompletedSource == null)
                _animationsCompletedSource = new TaskCompletionSource<bool>();

            await Task.WhenAny(_animationsCompletedSource.Task, exitTokenCompletion.Task);

            _animationsCompletedSource = null;
        }
        finally
        {
            exitTokenRegistration.Dispose();
        } 
    }

    public async Task PlayBuildingComplete()
    {
        var tasks = new List<Task>();

        for (var bitI = 0; bitI < bits.Length; bitI++)
        {
            tasks.Add(bits[bitI].PlayComplete(bitI));
            //Debug.Log($"Bit {bitI}");
        }

        await Task.WhenAll(tasks);
    }
}
