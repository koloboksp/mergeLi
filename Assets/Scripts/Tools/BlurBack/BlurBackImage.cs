
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    [SerializeField] private Image image;
    private GameObject go;

    private void Awake()
    {
        var rt = BlurBackHub.GetImage();

        if (image == null || rt == null)
            return;

        go = image.gameObject;
        
        DestroyImmediate(image);

        var rawImage = go.AddComponent<RawImage>();
        rawImage.texture = rt;

        var btn = go.GetComponent<Button>();
        if (btn != null)
            btn.targetGraphic = rawImage;
    }

    private async void OnEnable()
    {
        go.SetActive(false);

        BlurBackHub.UpdateImage();

        // Wait while Get Image before Window open
        var frame = Time.frameCount;
        while (Time.frameCount == frame)
            await Task.Yield();

        go.SetActive(true);
    }
}
