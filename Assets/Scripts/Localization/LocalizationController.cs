using System;
using System.Collections.Generic;
using System.Linq;
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
        private const SystemLanguage DefaultLanguage = SystemLanguage.English;
        
        private static readonly List<ILocalizationSupport> LocalizationSupportObjects = new();
        private static List<KeyValuePair<SystemLanguage, LanguagePack>> AvailableLanguagePacks = new();
        
        private static SystemLanguage _activeLanguage = DefaultLanguage;
        private static bool _dataLoaded;
        
        public async Task InitializeAsync(CancellationToken cancellationToken)
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

        public void SetLanguage(SystemLanguage language)
        {
            if (_activeLanguage == language)
                return;

            _activeLanguage = HasLanguage(language) ? language : DefaultLanguage;
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
        
        public IEnumerable<SystemLanguage> Languages
        {
            get
            {
                return AvailableLanguagePacks.Select(i => i.Key);
            }
        }

        public SystemLanguage ActiveActiveLanguage => _activeLanguage;

        public static bool HasLanguage(SystemLanguage language)
        {
            return GetPackIndex(language) >= 0;
        }

        public static int GetPackIndex(SystemLanguage language)
        {
            return AvailableLanguagePacks.FindIndex(i => i.Key == language);
        }

        public Sprite GetIcon(SystemLanguage language)
        {
            var foundPairIndex = AvailableLanguagePacks.FindIndex(i => i.Key == language);
            if (foundPairIndex >= 0)
                return AvailableLanguagePacks[foundPairIndex].Value.NativeFlag;

            return null;
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
            
            var index = GetPackIndex(_activeLanguage);
            if (index >= 0)
                return AvailableLanguagePacks[index].Value.Asset.FindText(guid) != null;

            return false;           
        }
        
        public static string GetText(Atom.GuidEx guid)
        {
            return GetText(_activeLanguage, guid);
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