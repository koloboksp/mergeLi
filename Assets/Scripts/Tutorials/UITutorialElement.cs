using System.Linq;
using UnityEngine;

namespace Core.Tutorials
{
    public class UITutorialElement : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private RectTransform _root;

        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }

        public RectTransform Root => _root;

        public static UITutorialElement FindByTag(string tag)
        {
            var tutorialElements = FindObjectsOfType<UITutorialElement>();
            var result = tutorialElements.FirstOrDefault(i => i.Tag == tag);

            if (result == null)
                Debug.LogError($"UITutorialElement with tag {tag} not found.");

            return result;
        }
    }
}