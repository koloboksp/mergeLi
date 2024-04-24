using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CastleViewer2 : MonoBehaviour
{
    private const float MASK_SCALE = .5f; // Relative to main image
    private const int PIX_PADDING = 4;

    [SerializeField] private Image image;
    [SerializeField] private ImagePattern pattern;
    [SerializeField] private CastleViewerPreset preset;

    [SerializeField] private int maxScore = 1000;
    [SerializeField] private int nowScore = 0;
     
    private CastleBit[] bits;

    // Debug
    private readonly int[] testScore = new int[] { 100, 500, 1000, -100, -500, -1000 };

    private void Awake()
    {
        MakeCastleBits();
        CalcPrices();

        image.enabled = false;

        AddScore(0);
    }

    private void MakeCastleBits()
    {
        bits = new CastleBit[pattern.bits.Count];

        RectTransform baseRectTrans = GetComponent<RectTransform>();
        var maskPixToBaseRect = new Vector2(
               1f / MASK_SCALE * baseRectTrans.sizeDelta.x / pattern.image.width,
               1f / MASK_SCALE * baseRectTrans.sizeDelta.y / pattern.image.height);

        // castleBits = new List<RectTransform>();
        // rTexs = new List<RenderTexture>();
        
        var matVertColor = new Material(preset.shVertColor);
        var matMultAlpha = new Material(preset.shMultAlpha);
        var matBlur = new Material(preset.shBlur);

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
            RenderTexture.active = rTexTemp;
            
            matVertColor.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, 1, 1, 0);

            GL.Clear(true, true, Color.black);
            GL.Begin(GL.TRIANGLES);
            GL.Color(Color.white);
            for (int j = 0; j < tris.Length; j++)
                GL.Vertex(verts[i][tris[j]]);
            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;

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
            var rTexBlur = RenderTexture.GetTemporary(pw, ph, 0, RenderTextureFormat.R8);
            Graphics.Blit(rTex, rTexBlur, matBlur);
            Graphics.Blit(rTexBlur, rTex, matBlur);
            RenderTexture.ReleaseTemporary(rTexBlur);

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

            var img = newObj.AddComponent<Image>();
            img.material = matBit;

            bits[i] = new CastleBit(rTrans, rTex, matBit);
        }
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

    private void OnDestroy()
    {
        for (int i = 0; i < bits.Length; i++)
            bits[i].Destroy();
    }

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
    private void OnGUI()
    {
        GUI.BeginGroup(new Rect(20, 20, 200, 500));

        for (int i = 0; i < testScore.Length; i++)
            if (GUILayout.Button("Add " + testScore[i]))
                AddScore(testScore[i]);

        GUI.EndGroup();
    }
}
