using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HorizontalRatioLayoutCorrector : MonoBehaviour
{
    [SerializeField] public int _referenceWidth = 768;
    [SerializeField] public int _referenceHeight = 1024;

    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasScaler _canvasScaler;

    public void Update()
    {
        var ratio = (float)Screen.width / Screen.height;
        var referenceRatio = (float)_referenceWidth / _referenceHeight;
        
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