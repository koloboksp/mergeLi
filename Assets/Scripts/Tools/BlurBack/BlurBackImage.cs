
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    private const float SHOT_SCALE = .2f;
    private const int BLUR_COUNT = 5;

    private readonly string matName = "matBlurBack";
    private readonly string rtName = "rtBlurBack";
    private readonly string shaderName = "Hidden/DropBlurBlit";
    private readonly string layerName = "UIPopup";

    private static RenderTexture s_rt;
    private static Material s_mat;

    [SerializeField] private Image image;
    
    private void Awake()
    {
        if (image == null)
            return;

        if (s_rt == null)
            s_rt = new RenderTexture((int)(Screen.width * SHOT_SCALE), (int)(Screen.height * SHOT_SCALE), 16)
            {
                name = rtName
            };

        if (s_mat == null)
            s_mat = new Material(Shader.Find(shaderName))
            {
                name = matName
            };

        var go = image.gameObject;
        
        DestroyImmediate(image);

        var rawImage = go.AddComponent<RawImage>();
        rawImage.texture = s_rt;
        rawImage.color = Color.white;

        var btn = go.GetComponent<Button>();
        if (btn != null)
            btn.targetGraphic = rawImage;
    }

    private void OnEnable()
    {
        var cam = Camera.main;
        int layer = 1 << LayerMask.NameToLayer(layerName);
        cam.cullingMask &= ~layer;

        cam.targetTexture = s_rt;
        cam.Render();
        cam.targetTexture = null;

        cam.cullingMask |= layer;

        LazyBlur();
    }

    private async void LazyBlur()
    {
        int counter = BLUR_COUNT;
        var rtTemp = RenderTexture.GetTemporary(s_rt.width, s_rt.height, s_rt.depth, s_rt.format);

        while (counter > 0)
        {
            Graphics.Blit(s_rt, rtTemp, s_mat);
            Graphics.Blit(rtTemp, s_rt, s_mat);

            counter--;

            await Task.Yield();

            if (!Application.isPlaying)
                break;
        }

        rtTemp.Release();
    }
}
