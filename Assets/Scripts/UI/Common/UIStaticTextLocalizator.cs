using Assets.Scripts.Core.Localization;
using UnityEngine;
using UnityEngine.UI;
using Atom;
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

        public void UpdateText(string text = null)
        {
            if (Target == null)
                return;

            if(text != null)
                Target.text = text;
            else
            {
		        var t = LocalizationController.GetText(_id);
                Target.text = t;
	        }
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