#if DEBUG
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICheatsPanel_DropdownItem : MonoBehaviour
    {
        [SerializeField] private Dropdown _inputField;
        [SerializeField] private Text _name;
        [SerializeField] private GameObject _selectionFrame;
        
        private DropdownHolder _model;
        
        private void Awake()
        {
            _inputField.onValueChanged.AddListener(OnSubmit);
        }
        
        public void SetModel(DropdownHolder model)
        {
            _model = model;

            _name.text = model.Id;
            _inputField.options = model.Items
                .Select(i => new Dropdown.OptionData() { text = i })
                .ToList();

            _inputField.SetValueWithoutNotify(model.SelectedIndex);
        }
        
        private void OnSubmit(int selectedIndex)
        {
            _model.SelectedIndex = selectedIndex;
        }
    }
}
#endif