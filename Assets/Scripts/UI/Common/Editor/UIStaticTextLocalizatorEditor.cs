using Assets.Scripts.Core;
using Assets.Scripts.Core.Localization;
using UnityEditor;

namespace Assets.Scripts.CustomLevelElements
{
    [CustomEditor(typeof(UIStaticTextLocalizator), true)]
    public class UIStaticTextLocalizatorEditor : UnityEditor.Editor
    { 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var localizator = target as UIStaticTextLocalizator;

            if (localizator.Id != Atom.GuidEx.Empty)
            {
                for (var i = 0; i != LocalizationController.LanguagesCount; i++)
                {
                    var languagePackDesc = LocalizationController.GetPackDesc(i);
                    var langText = LocalizationController.GetText(languagePackDesc.Language, localizator.Id);
   
                    EditorGUILayout.LabelField($"{languagePackDesc.Language}: ", langText);
                }
            }
        }
    }
}