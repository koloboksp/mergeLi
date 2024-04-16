using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastleCutter : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Texture2D mask;
    [SerializeField] private Material matSelector;
    [SerializeField] private Material matBlur;
    [SerializeField] private Material matGradients;
    [SerializeField] private Material matShow;

 
    [Space(10)]
    [SerializeField] private float stageStart = 248;
    [SerializeField] private float stageStep = -8;
    [SerializeField] private Transform[] stagePoints;

    private List<Material> mats;
    private List<RenderTexture> rts;

    private void Awake()
    {
        MaskBits();
    }

    private void MaskBits()
    {
        ReleaseRenderTextures();

        rts = new List<RenderTexture>();
        mats = new List<Material>();

        var imgTrans = img.GetComponent<RectTransform>();

        var mSelector = new Material(matSelector);

        int w0 = mask.width;
        int h0 = mask.height;

        int w = mask.width * 2;
        int h = mask.height * 2;
        
        var rtTemp0 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.R8);
        var rtTemp1 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.R8);

        float stageValue = stageStart;

        for (int i = 0; i < stagePoints.Length; i++)
        {
            mSelector.SetFloat("_Step", stageValue / 255f);
            stageValue += stageStep;

            // Work with full size render textures
            Graphics.Blit(mask, rtTemp0, mSelector);
            Graphics.Blit(rtTemp0, rtTemp1, matBlur);
            Graphics.Blit(rtTemp1, rtTemp0, matBlur);

            // Copy Only not black pixels
            var iRect = GetRectInt(rtTemp0);
            var rtCrop = RenderTexture.GetTemporary(iRect.width, iRect.height, 0, RenderTextureFormat.R8);
            Graphics.CopyTexture(rtTemp0, 0, 0, iRect.x, iRect.y, iRect.width, iRect.height, rtCrop, 0, 0, 0, 0);

            // Make Other Gradients
            var rt = new RenderTexture(iRect.width, iRect.height, 0, RenderTextureFormat.ARGB32, 0) { name = "CastleBit_" + i };
            Graphics.Blit(rtCrop, rt, matGradients);
            RenderTexture.ReleaseTemporary(rtCrop);

            rts.Add(rt);

            var newGo = new GameObject("img");
            var newTrans = newGo.AddComponent<RectTransform>();
            newTrans.parent = transform;
            newTrans.localScale = Vector3.one;
            newTrans.anchoredPosition = Vector2.zero;
            newTrans.sizeDelta = imgTrans.sizeDelta;
            newTrans.parent = stagePoints[i];
            
            var newImg = newGo.AddComponent<Image>();
            newImg.sprite = img.sprite;

            var newMat = new Material(matShow);
            newImg.material = newMat;
            newMat.SetTexture("_Mask", rt);
            newMat.SetVector("_Mask_ST", new Vector4(
                w / (float)iRect.width, h / (float)iRect.height,
                iRect.x / (float)w, iRect.y / (float)h));
            mats.Add(newMat);
        }

        RenderTexture.ReleaseTemporary(rtTemp0);
        RenderTexture.ReleaseTemporary(rtTemp1);
    }

    private RectInt GetRectInt(RenderTexture rt)
    {
        int w = rt.width;
        int h = rt.height;

        var rtBuf = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(rt.width, rt.height, TextureFormat.R8, false, true);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
        tex.Apply();
        RenderTexture.active = rtBuf;

        var bytes = tex.GetRawTextureData();
        int minx = w;
        int miny = h;
        int maxx = 0;
        int maxy = 0;

        for (int x = 0; x < w; x += 2)
        {
            for (int y = 0; y < h; y += 2)
            {
                if (bytes[y * w + x] == 0)
                    continue;

                if (x < minx) minx = x;
                if (x > maxx) maxx = x;
                if (y < miny) miny = y;
                if (y > maxy) maxy = y;
            }
        }

        int padding = 2;
        minx -= padding;
        miny -= padding;
        maxx += padding;
        maxy += padding;

        return new RectInt(minx, miny, maxx - minx, maxy - miny);
    }

    private void OnDestroy()
    {
        ReleaseRenderTextures();
    }

    private void ReleaseRenderTextures()
    {
        if (rts != null && rts.Count > 0)
            foreach (var item in rts)
                item.Release();
    }
}
