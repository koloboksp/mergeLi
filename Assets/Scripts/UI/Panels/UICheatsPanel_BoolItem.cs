using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICheatsPanel_BoolItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _stateIcon;
        [SerializeField] private Sprite _checkedIcon;
        [SerializeField] private Sprite _uncheckedIcon;

        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selectionFrame;
        
        private BoolHolder _model;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(BoolHolder model)
        {
            _model = model;
            
            _name.text = _model.Id;
            SetStateIcon(_model.Value);
        }
        
        private void OnClick()
        {
            var value = !_model.Value;
            
            SetStateIcon(value);
            _model.ChangeValue(value);
        }

        private void SetStateIcon(bool value)
        {
            _stateIcon.sprite = value ? _checkedIcon : _uncheckedIcon;
        }
    }
}