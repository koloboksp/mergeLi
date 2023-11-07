
using UnityEngine;

public class BlurBack : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Shader blurShader;
    [SerializeField] private RenderTexture rtHard;
    [SerializeField] private RenderTexture rtBlur;

    private Material mat;


    private void Apply()
    {
        if (cam == null || blurShader == null || rtHard == null || rtBlur == null)
            return;
        
        cam.targetTexture = rtHard;
        cam.Render();
        cam.targetTexture = null;

        if (mat == null)
            mat = new Material(blurShader);

        for (int i = 0; i < 5; i++)
        {
            Graphics.Blit(rtHard, rtBlur, mat);
            Graphics.Blit(rtBlur, rtHard);
        }
    }

    private void OnEnable()
    {
        Apply();
    }
}
