
using UnityEngine;
using UnityEngine.UI;

public class BlurBackImage : MonoBehaviour
{
    private readonly Color COLOR = new(0.3f, 0.3f, 0.3f, 1f);
    
    [SerializeField] private Image image;

    private void Awake()
    {
        var rt = BlurBackHub.GetImage();

        if (image == null || rt == null)
            return;

        var go = image.gameObject;
        
        DestroyImmediate(image);

        var rawImage = go.AddComponent<RawImage>();
        // rawImage.color = COLOR;
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
