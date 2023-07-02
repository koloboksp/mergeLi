using UnityEngine;
using UnityEngine.EventSystems;

public class Buff : MonoBehaviour
{
    private GameObject _cursorPrefab;
    private GameObject _cursor;

    [SerializeField] protected GameProcessor _gameProcessor;
    [SerializeField] private UIBuff _controlPrefab;

    private UIBuff _control;

    void OnDragStarted()
    {
        CreateCursor();
    }

    private void CreateCursor()
    {
     //   _cursor = Instantiate(_cursorPrefab, CursorPosition);
    }

    public Vector3 CursorPosition { get; set; }
    public UIBuff Control => _control;
    
    private void UpdateCursorPosition()
    {
        _cursor.transform.position = CursorPosition;
    }

    public UIBuff CreateControl()
    {
        _control = Instantiate(_controlPrefab);
        _control
            .OnClick(OnClick)
            .OnBeginDrag(OnBeginDrag)
            .OnEndDrag(OnEndDrag)
            .OnDrag(OnDrag);
        return _control;
    }
    
    protected virtual void OnClick()
    {
        
    }
    
    protected virtual void OnEndDrag(PointerEventData eventData)
    {
        
    }

    protected virtual void OnBeginDrag(PointerEventData eventData)
    {
        
    }
    
    protected virtual void OnDrag(PointerEventData eventData)
    {
        
    }
}