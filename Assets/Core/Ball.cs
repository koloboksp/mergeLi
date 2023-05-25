using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ball : MonoBehaviour, IFieldMovable, IFieldSelectable, IFieldMergeable
{
    [SerializeField] private BallSelectionEffect _selectionEffect;
    private int _points = 1;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Field _field;
    private float _moveSpeed = 15.0f;
    [SerializeField] private RectTransform _root;
    [SerializeField] private Text _indexLabel;
    [SerializeField] private Text _valueLabel;
    [SerializeField] private Image _ballIcon;

    public static Dictionary<int, Color> _colors = new Dictionary<int,Color>()
    {
        {1, Color.yellow},
        {2, Color.green},
            {4, Color.magenta},
            {8, Color.red},
            {16, Color.white},
            {32, Color.cyan},
            {64, Color.cyan},
            {128, Color.cyan},
            {256, Color.cyan},
    };
    
    public Vector3 Position => _position;
    
    public Vector3Int IntPosition => new Vector3Int(Mathf.FloorToInt(_position.x), Mathf.FloorToInt(_position.y));

    public void Awake()
    {
        _points = 1;
        UpdateView();
    }
    
    public IEnumerator InnerMove(Vector3Int to, Action<bool> onComplete)
    {
        var path = _field.GetPath(new Vector3Int((int)_position.x, (int)_position.y), to);
        var pathFound = path.Count > 0;
        if (pathFound)
        {
            bool moving = true;
        
            float timer = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var startP = new Vector3(path[i].x, path[i].y);
                var endP = new Vector3(path[i + 1].x, path[i + 1].y);
                var pathVec = endP - startP;
                var pathLength = pathVec.magnitude;
                var moveTime = pathLength / _moveSpeed;
            
                while (timer <= moveTime)
                {
                    timer += Time.deltaTime;
                    var newPosition = (startP + Vector3.Lerp(Vector3.zero, pathVec, timer / moveTime));
                    UpdateTransformPosition(newPosition);
                
                    yield return null;
                }
                timer -= moveTime;
            }
        
            UpdateTransformPosition(new Vector3(to.x, to.y));
        }
        
        onComplete?.Invoke(pathFound);
    }
    
    public IEnumerator InnerMerge(IEnumerable<IFieldMergeable> others, Action onComplete)
    {
        foreach (var other in others)
            _points += other.Points;

        foreach (var other in others)
            _field.DestroySomething(other as Ball);

        yield return null;
        
        UpdateView();
        
        onComplete?.Invoke();
    }

    public void SetData(Field field, Vector3 startPosition, int points)
    {
        _field = field;
        _points = points;
        
        UpdateTransformPosition(startPosition);
        UpdateView();
    }

    void UpdateTransformPosition(Vector3 position)
    {
        _root.anchoredPosition = _field.GetPosition(position);
        _position = position;
        if(_indexLabel != null)
            _indexLabel.text = $"{_position.x}:{_position.y}";
    }

    private void UpdateView()
    {
        if(_colors.ContainsKey(_points))
            _ballIcon.color = _colors[_points];
        _valueLabel.text = _points.ToString();
    }

    public void StartMove(Vector3Int endPosition, Action<IFieldMovable, bool> onMovingComplete)
    {
        StartCoroutine(InnerMove(endPosition, OnComplete));

        void OnComplete(bool pathFound)
        {
            onMovingComplete?.Invoke(this, pathFound);
        }
    }
    
    public void StartMerge(IEnumerable<IFieldMergeable> others, Action<IFieldMergeable> onMergeComplete)
    {
        StartCoroutine(InnerMerge(others, OnComplete));

        void OnComplete()
        {
            onMergeComplete?.Invoke(this);
        }
    }

    public int Points => _points;

    public void Select(bool select)
    {
        _selectionEffect.SetActiveState(select);
    }
}