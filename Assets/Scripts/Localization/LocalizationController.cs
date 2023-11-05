using System;
using System.Collections.Generic;
using System.Threading;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;
using System.Threading.Tasks;
using Atom.Timers;
using Core;

namespace Assets.Scripts.Core.Localization
{
    public interface ILanguage
    {
        SystemLanguage Language { get; set; }
    }

    public interface ILocalizationSupport
    {
        void ChangeLocalization();
    }

    [Serializable]
    public class LocalizationItem
    {
        public SystemLanguage Language;
        public string NativeName;
        public Sprite NativeFlag;  
        public TextAsset Asset;
    }
    
    public class LocalizationController 
    {
        public const string TextNotFoundValue = "Text not found";
        private const SystemLanguage mDefaultLanguage = SystemLanguage.English;
        
        private static readonly List<ILocalizationSupport> LocalizationSupportObjects = new();
        private static List<KeyValuePair<SystemLanguage, LanguagePack>> AvailableLanguagePacks = new();
        
        private static SystemLanguage mLanguage = mDefaultLanguage;
        private static bool _dataLoaded;
        
        public async Task InitializeAsync(ILanguage storage, CancellationToken cancellationToken)
        {
            _dataLoaded = false;
            AvailableLanguagePacks = await LocalizationData.LoadAsync(cancellationToken);
            _dataLoaded = true;
          
            ChangeLocalization();
        }

        public static void Add(ILocalizationSupport listener)
        {
            LocalizationSupportObjects.Add(listener);

            try
            {
                listener.ChangeLocalization();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void Remove(ILocalizationSupport listener)
        {
            LocalizationSupportObjects.Remove(listener);
        }

        public static SystemLanguage GetLanguage()
        {
            return mLanguage;
        }

        public static void SetLanguage(SystemLanguage language)
        {
            if (mLanguage == language)
                return;

            mLanguage = HasLanguage(language) ? language : mDefaultLanguage;
            ChangeLocalization();
        }

        private static void ChangeLocalization()
        {
            for (var i = 0; i != LocalizationSupportObjects.Count; i++)
            {
                var localizationSupportObject = LocalizationSupportObjects[i];

                try
                {
                    localizationSupportObject.ChangeLocalization();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static int LanguagesCount => AvailableLanguagePacks.Count;

        public static bool HasLanguage(SystemLanguage language)
        {
            return GetPackIndex(language) >= 0;
        }

        public static int GetPackIndex(SystemLanguage language)
        {
            return AvailableLanguagePacks.FindIndex(i => i.Key == language);
        }

        public static LanguagePackDesc GetPackDesc(int i)
        {
            var languagePack = AvailableLanguagePacks[i];

            return new LanguagePackDesc(languagePack.Key, languagePack.Value.NativeName, languagePack.Value.NativeFlag);
        }
        
        public static string GetText(SystemLanguage language, Atom.GuidEx guid)
        {
            if (!_dataLoaded)
            {
                AvailableLanguagePacks = LocalizationData.Load();
                _dataLoaded = true;
            }
            
            var index = GetPackIndex(language);
            
            if (index < 0) 
                return $"{TextNotFoundValue}({guid})";

            var text = AvailableLanguagePacks[index].Value.Asset.FindText(guid);

            return text ?? $"{TextNotFoundValue}({guid})";
        }

        public static bool HasText(GuidEx guid)
        {
            if (!_dataLoaded)
            {
                AvailableLanguagePacks = LocalizationData.Load();
                _dataLoaded = true;
            }
            
            var index = GetPackIndex(mLanguage);
            if (index >= 0)
                return AvailableLanguagePacks[index].Value.Asset.FindText(guid) != null;

            return false;           
        }
        
        public static string GetText(Atom.GuidEx guid)
        {
            return GetText(mLanguage, guid);
        }

        public readonly struct LanguagePackDesc
        {
            public readonly SystemLanguage Language;
            public readonly string NativeName;
            public readonly Sprite NativeFlag;

            public LanguagePackDesc(SystemLanguage packKey, string nativeName, Sprite nativeFlag)
            {
                Language = packKey;
                NativeName = nativeName;
                NativeFlag = nativeFlag;
            }
        }
    }
}