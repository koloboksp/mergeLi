using UnityEngine;
using UnityEngine.UI;

public class FieldView : MonoBehaviour, IFieldView
{
    [SerializeField] private Field _model;
    [SerializeField] private RectTransform _root;
    
    [SerializeField] private Color _gridColor = Color.gray;
    [SerializeField] private float _gridThickness = 8.0f;

    public Transform Root => _root;
    public Vector2 Size => _root.rect.size;

    public void RegenerateField()
    {
        var size = _root.rect.size;

        for (int x = 0; x < _model.Size.x; x++)
        {
            GameObject line = new GameObject($"line_{x}", typeof(RectTransform));

            var image = line.AddComponent<Image>();
            image.color = _gridColor;

            var lineTransform = line.GetComponent<RectTransform>();
            lineTransform.pivot = Vector2.zero;
            lineTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _gridThickness);
            lineTransform.anchoredPosition = new Vector2(x * CellSize().x, 0);
        }
        
        for (int y = 0; y < _model.Size.x; y++)
        {
            GameObject line = new GameObject($"line_{y}", typeof(RectTransform));

            var image = line.AddComponent<Image>();
            image.color = _gridColor;

            var lineTransform = line.GetComponent<RectTransform>();
            lineTransform.pivot = Vector2.zero;
            lineTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _gridThickness);
            lineTransform.anchoredPosition = new Vector2(y * CellSize().x, 0);
        }
    }

    public Vector3 CellSize()
    {
        return new Vector3(_root.rect.size.x / _model.Size.x, _root.rect.size.x / _model.Size.y, 0);
    }
}