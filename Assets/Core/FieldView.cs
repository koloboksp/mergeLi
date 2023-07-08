using UnityEngine;
using UnityEngine.UI;

public class FieldView : MonoBehaviour, IFieldView
{
    [SerializeField] private Field _model;
    [SerializeField] private RectTransform _root;
    [SerializeField] private RectTransform _backgroundRoot;

    [SerializeField] private Color _gridColor = Color.gray;
    [SerializeField] private float _gridThickness = 8.0f;

    public Transform Root => _root;
    public Vector2 SizeDelta => _root.sizeDelta;

    public void RegenerateField()
    {
        for (int x = 0; x < _model.Size.x; x++)
        {
            GameObject line = new GameObject($"line_{x}", typeof(RectTransform));

            var image = line.AddComponent<Image>();
            image.color = _gridColor;

            var lineTransform = line.GetComponent<RectTransform>();
            lineTransform.SetParent(_backgroundRoot);
            lineTransform.pivot = Vector2.zero;
            lineTransform.anchorMin = Vector2.zero;
            lineTransform.anchorMax = Vector2.zero;
            lineTransform.sizeDelta = new Vector2(_gridThickness, SizeDelta.y);
            lineTransform.anchoredPosition = new Vector2(x * CellSize().x, 0);
            lineTransform.localScale = Vector3.one;
        }
        
        for (int y = 0; y < _model.Size.x; y++)
        {
            GameObject line = new GameObject($"line_{y}", typeof(RectTransform));

            var image = line.AddComponent<Image>();
            image.color = _gridColor;

            var lineTransform = line.GetComponent<RectTransform>();
            lineTransform.SetParent(_backgroundRoot);
            lineTransform.pivot = Vector2.zero;
            lineTransform.anchorMin = Vector2.zero;
            lineTransform.anchorMax = Vector2.zero;
            lineTransform.sizeDelta = new Vector2(SizeDelta.x, _gridThickness);
            lineTransform.anchoredPosition = new Vector2(0, y * CellSize().y);
            lineTransform.localScale = Vector3.one;
        }
    }

    public Vector3 CellSize()
    {
        return new Vector3(_root.rect.size.x / _model.Size.x, _root.rect.size.x / _model.Size.y, 0);
    }
}