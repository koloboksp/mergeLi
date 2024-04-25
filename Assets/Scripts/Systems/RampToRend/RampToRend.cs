using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampToRend : MonoBehaviour
{
    private const int SIZE = 32;
    private const string PROP_NAME = "_RampMap";

    [SerializeField] private Renderer rend;
    [SerializeField] private Gradient grad;

    private Color32[] cols;
    private Texture2D tex;
    public Texture2D Tex
    {
        get
        {
            if (tex == null)
                MakeMap();

            return tex;
        }
    }

    private void Awake()
    {
        if (rend == null)
            return;

        MakeMap();
        ApplyRamp();
    }

    private void MakeMap()
    {
        tex = new Texture2D(SIZE, 1, TextureFormat.RGBA32, false, true);
        cols = new Color32[SIZE];

        float fSize = (float)SIZE;
        for (int i = 0; i < SIZE; i++)
            cols[i] = grad.Evaluate(i / fSize);

        tex.SetPixels32(cols);
        tex.Apply();
    }

    private void ApplyRamp()
    {
        var mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);
        mpb.SetTexture(PROP_NAME, tex);
        rend.SetPropertyBlock(mpb);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rend == null)
            return;

        MakeMap();
        ApplyRamp();
    }

    public void ClearPropertyBlock()
    {
        if (rend == null)
            return;

        if (rend.HasPropertyBlock())
            rend.SetPropertyBlock(null);

        rend = null;
    }
#endif
}
