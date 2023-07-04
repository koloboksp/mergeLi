using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.OrientationMutators
{
    [ExecuteInEditMode]
    public class UIOrientationMutator : MonoBehaviour
    {
        [SerializeField] private List<UIMutator> _mutators = new List<UIMutator>();

        private ScreenOrientation _screenOrientation;

        private void Awake()
        {
            _screenOrientation = ScreenOrientation;
            UpdateOrientation(_screenOrientation);
        }
        
        private void Update()
        {
            if (_screenOrientation != ScreenOrientation)
            {
                _screenOrientation = ScreenOrientation;
                UpdateOrientation(_screenOrientation);
            }
        }
        
        public void OnValidate()
        {
            UpdateOrientation(ScreenOrientation);
        }

        public ScreenOrientation ScreenOrientation 
        {
            get
            {
#if UNITY_EDITOR
                return Screen.height > Screen.width ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
#else
                return Screen.orientation;
#endif
            }
        }
        
        private void UpdateOrientation(ScreenOrientation orientation)
        {
            foreach (UIMutator mutator in _mutators)
            {
                mutator.Apply(orientation);
            }
        }
    }
}