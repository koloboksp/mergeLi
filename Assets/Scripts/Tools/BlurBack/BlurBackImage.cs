
using UnityEngine;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    [SerializeField] private Image image;


    private void Awake()
    {
        var rt = BlurBackHub.GetImage();

        if (image == null || rt == null)
            return;

        var go = image.gameObject;

        DestroyImmediate(image);

        var rawImage = go.AddComponent<RawImage>();
        rawImage.texture = rt;

        var btn = go.GetComponent<Button>();
        if (btn != null)
            btn.targetGraphic = rawImage;
    }

    private void OnEnable()
    {
        BlurBackHub.UpdateImage();
    }
}
