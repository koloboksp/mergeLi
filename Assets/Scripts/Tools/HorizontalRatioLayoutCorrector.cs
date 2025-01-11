using UnityEngine;
using UnityEngine.UI;

public class HorizontalRatioLayoutCorrector : MonoBehaviour
{
    public int Width = 1080;
    public int Height = 1920;

    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasScaler _canvasScaler;

    public void Update()
    {
        var ratio = (float)Screen.width / Screen.height;
        var referenceRatio = (float)Width / Height;
        
        if (ratio > referenceRatio)
        {
            float width = 1.0f * (referenceRatio /ratio );
            float offset = (1.0f - width) / 2.0f;
            _camera.rect = new Rect(offset, 0, width, 1);
        }
        else
        {
            _camera.rect = new Rect(0, 0, 1, 1);
        }
    }
}