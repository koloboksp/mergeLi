#if DEBUG
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICheatsPanel_IntItem : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selectionFrame;
        
        private IntHolder _model;
        
        private void Awake()
        {
            _inputField.onSubmit.AddListener(OnSubmit);
        }
        
        public void SetModel(IntHolder model)
        {
            _model = model;
            
            _name.text = _model.Id;
            _inputField.text = _model.Value.ToString();
        }
        
        private void OnSubmit(string textValue)
        {
            if (int.TryParse(textValue, out var value))
            {
                _model.ChangeValue(value);
            }
        }
    }
}
#endif