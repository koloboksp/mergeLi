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
        public GuidEx Id;

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
		        var t = LocalizationController.GetText(Id);
                Target.text = t;
	        }
        }

        public void SetText(GuidEx id)
        {
            Id = id;
            UpdateText();
        }
    }
}