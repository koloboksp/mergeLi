using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HorizontalCanvasScalerCorrector : MonoBehaviour
{
    [SerializeField] private CanvasScaler _canvasScaler;

    public void Update()
    {
        _canvasScaler.matchWidthOrHeight = (Screen.width > Screen.height) ? 0 : 1;
    }
}