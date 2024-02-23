using Assets.Scripts.Core.Localization;
using UnityEngine;
using UnityEngine.UI;
using Atom;
using Core;
using UnityEngine.Serialization;

namespace Assets.Scripts.Core
{
    public class UIStaticTextLocalizator : MonoBehaviour, ILocalizationSupport
    {
        public Text Target;
        [SerializeField]
        [FormerlySerializedAsAttribute("mGuid")]
        private GuidEx _id;

        void Awake()
        {
            LocalizationController.Add(this);   
        }

        void OnDestroy()
        {
            LocalizationController.Remove(this);
        }

        public void ChangeLocalization()
        {
            UpdateText();
        }

        void OnValidate()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (Target == null)
                return;
            
            var text = string.Empty;
            if (Application.isPlaying)
                text = ApplicationController.Instance.LocalizationController.GetText(_id);
            else
                text = LocalizationController.GetTextInEditorMode(LocalizationController.DefaultLanguage, _id);

            Target.text = text;
        }

        public GuidEx Id
        {
            set
            {
                _id = value;
                UpdateText();
            }
            get
            {
                return _id;
            }
        }
    }
}