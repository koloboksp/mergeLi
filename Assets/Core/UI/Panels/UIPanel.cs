﻿using System;
using UnityEngine;

namespace Core
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public RectTransform Root => _root;
        
        public virtual void SetData(UIScreenData data)
        {
            
        }

        public void Activate()
        {
            InnerActivate();
        }
        protected virtual void InnerActivate()
        {
            
        }
        
        public void Hide()
        {
            InnerHide();
        }

        protected virtual void InnerHide()
        {
            
        }

        public void Deactivate()
        {
            
        }
    }

    public class UIScreenData
    {
        
    }
}