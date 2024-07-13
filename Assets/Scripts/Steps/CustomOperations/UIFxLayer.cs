using System;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UIFxLayer : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public RectTransform Root => _root;
    }
}