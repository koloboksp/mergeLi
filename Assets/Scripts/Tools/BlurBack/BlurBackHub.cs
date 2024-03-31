
using System.Threading.Tasks;
using UnityEngine;

public class BlurBackHub : MonoBehaviour
{
    private const float SHOT_SCALE = .2f;
    private const int BLUR_COUNT = 5;
    
    [SerializeField] private Camera cam;
    [SerializeField] private Shader blurShader;

    private RenderTexture rt;
    private Material mat;

    private static BlurBackHub s_instance;

    public static void UpdateImage()
    {
        if (s_instance != null)
            s_instance.enabled = true;
    }

    public static RenderTexture GetImage() 
    {
        if (s_instance == null)
            return null;

        return s_instance.rt;
    }

    private void Awake()
    {
        if (cam == null || blurShader == null)
            return;

        mat = new Material(blurShader)
        {
            name = "matBlurBack"
        };

        rt = new RenderTexture((int)(Screen.width * SHOT_SCALE), (int)(Screen.height * SHOT_SCALE), 16)
        {
            name = "rtBlurBack"
        };
        
        s_instance = this;
        enabled = false;
    }

    private void OnDestroy() => s_instance = null;


    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, rt, mat);
        enabled = false;

        LazyBlur();

        Graphics.Blit(src, dst);
    }

    private async void LazyBlur()
    {
        int counter = BLUR_COUNT;
        var rtTemp = RenderTexture.GetTemporary(rt.width, rt.height, rt.depth, rt.format);

        while (counter > 0)
        {
            Graphics.Blit(rt, rtTemp, mat);
            Graphics.Blit(rtTemp, rt, mat);

            counter--;

            await Task.Yield();

            if (!Application.isPlaying)
                break;
        }

        rtTemp.Release();
    }
}
