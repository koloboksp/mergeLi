
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    private const float SHOT_SCALE = .2f;
    private const int BLUR_COUNT = 5;

    [SerializeField] private Material matBlur;
    [SerializeField] private RawImage image;

    private static Texture2D tex;
    private static Sprite sprite;

    private void OnEnable()
    {
        MakeBlurImage();
    }

    private void MakeBlurImage()
    {
        if (image == null || matBlur == null)
        {
           enabled = false;
           return;
        }
        
        image.enabled = false;
        image.color = Color.white;

        var cam = Camera.main;
        if (cam == null)
            return;

        int w = (int)(Screen.width * SHOT_SCALE);
        int h = (int)(Screen.height * SHOT_SCALE);
        var rect = new Rect(0, 0, w, h);

        if (tex == null || tex.width != w || tex.height != h)
            tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        
        image.texture = tex;
        image.uvRect = cam.rect;
        var rt0 = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32);
        var rt1 = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);

        cam.targetTexture = rt0;
        cam.Render();
        cam.targetTexture = null;

        for (int i = 0; i < BLUR_COUNT; i++)
        {
            Graphics.Blit(rt0, rt1, matBlur);
            Graphics.Blit(rt1, rt0, matBlur);
        }

        RenderTexture.active = rt0;
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt0);
        RenderTexture.ReleaseTemporary(rt1);

        image.enabled = true;
    }
}
