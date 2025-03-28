﻿using System.Collections.Generic;
using UnityEngine;

namespace Skins
{
    public class SkinsLibrary : MonoBehaviour
    {
        [SerializeField] private List<SkinContainer> _skinContainers;

        public IEnumerable<SkinContainer> Containers => _skinContainers;

        public SkinContainer GetContainer(string skinName)
        {
            return _skinContainers.Find(i => i.Name == skinName);
        }

        public SkinContainer GetDefaultSkin()
        {
            return _skinContainers.Find(i => i.Name == "default");
        }
    }
}