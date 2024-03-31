
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    [SerializeField] private Image image;
    private GameObject go;

    // Grab blur image only for first open window
    // Next can use this blured image
    private static int s_count;

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
        if (s_count == 0)
        {
            go.SetActive(false);

            BlurBackHub.UpdateImage();

            var frame = Time.frameCount + 1;
            while (Time.frameCount < frame)
                await Task.Yield();

            go.SetActive(true);
        }

        s_count++;
    }

    private void OnDisable()
    {
        s_count = s_count < 0 ? 0 : s_count - 1;
    }
}
