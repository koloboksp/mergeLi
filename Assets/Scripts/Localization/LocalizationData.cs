using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.Localization
{
    [CreateAssetMenu(menuName = "Create LocalizationData", fileName = "LocalizationData", order = 0)]
    public class LocalizationData : ScriptableObject
    {
        private static readonly List<KeyValuePair<SystemLanguage, LanguagePack>> mAvailableLanguagePacks = new();

        public List<LocalizationItem> Packs;

        public static async Task<List<KeyValuePair<SystemLanguage, LanguagePack>>> LoadAsync(CancellationToken cancellationToken)
        {
            var request = Resources.LoadAsync(nameof(LocalizationData));

            while (request.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (request.asset == null)
                throw new Exception($"Resource '{nameof(LocalizationData)}' not created.");

            var localizationData = request.asset as LocalizationData;

            if (localizationData == null)
                throw new Exception($"Resource '{nameof(LocalizationData)}' is not '{nameof(LocalizationData)}'.");

            var availableLanguagePacks = new List<KeyValuePair<SystemLanguage, LanguagePack>>();
            
            foreach (var item in localizationData.Packs)
            {
                availableLanguagePacks.Add(new KeyValuePair<SystemLanguage, LanguagePack>(
                    item.Language,
                    new LanguagePack(item.NativeName, item.NativeFlag, item.Asset)));
            }

            return availableLanguagePacks;
        }

        public static List<KeyValuePair<SystemLanguage, LanguagePack>> Load()
        {
            var request = Resources.Load(nameof(LocalizationData));
            var localizationData = request as LocalizationData;

            if (localizationData == null)
                throw new Exception($"Resource '{nameof(LocalizationData)}' is not '{nameof(LocalizationData)}'.");

            var availableLanguagePacks = new List<KeyValuePair<SystemLanguage, LanguagePack>>();
            
            foreach (var item in localizationData.Packs)
            {
                availableLanguagePacks.Add(new KeyValuePair<SystemLanguage, LanguagePack>(
                    item.Language,
                    new LanguagePack(item.NativeName, item.NativeFlag, item.Asset)));
            }

            return availableLanguagePacks;
        }
    }
    
    public class LanguagePack
    {
        public readonly string NativeName;
        public readonly Sprite NativeFlag;
        public Atom.Localization.LcText Asset;

        public LanguagePack(string nativeName, Sprite nativeFlag, TextAsset data)
        {
            NativeName = nativeName;
            NativeFlag = nativeFlag;
            Asset = new Atom.Localization.LcText(data.bytes);
        }
    }
}