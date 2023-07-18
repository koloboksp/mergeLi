
using System;
using UnityEngine;
using UnityEngine.UI;

public class CastlePartView : MonoBehaviour
{
    public event Action OnClick;

    [SerializeField] private RectTransform _root;
    [SerializeField] private CastlePart _model;
    [SerializeField] private Image _selectionFrame;
    [SerializeField] private Image _outlineImage;
    [SerializeField] private Image _image;
    [SerializeField] private Button _areaClick;

    public RectTransform Root => _root;
    
    private void Awake()
    {
        _areaClick.onClick.AddListener(Area_OnClick);
        _model.OnIconChanged += OnIconChanged;
        OnIconChanged();
        _model.OnCostChanged += OnCostChanged;
        OnCostChanged();
        _model.OnProgressChanged += OnProgressChanged;
        OnProgressChanged();
        _model.OnSelectedStateChanged += OnSelectedStateChanged;
        OnSelectedStateChanged();
    }
    
    private void Area_OnClick()
    {
        OnClick?.Invoke();
    }

    private void OnIconChanged()
    {
        _outlineImage.sprite = _model.Icon;
        _image.sprite = _model.Icon;
    }
    
    private void OnCostChanged()
    {
        OnProgressChanged();
    }
    
    private void OnSelectedStateChanged()
    {
        _selectionFrame.gameObject.SetActive(_model.Selected);
    }
    
    private void OnProgressChanged()
    {
        _image.fillAmount = (float)_model.Points / (float)_model.Cost;
    }
}