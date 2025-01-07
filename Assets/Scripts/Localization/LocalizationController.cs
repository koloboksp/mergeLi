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
using Save;
using SmallTimer = Atom.Timers.SmallTimer;

namespace Assets.Scripts.Core.Localization
{
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
        public const string LanguagePackNotFoundValue = "Language pack not found";
        public const string TextNotFoundValue = "Text not found";
        public const SystemLanguage DefaultLanguage = SystemLanguage.English;
        
        private static readonly List<ILocalizationSupport> LocalizationSupportObjects = new();
        private static List<KeyValuePair<SystemLanguage, LanguagePack>> AvailableLanguagePacks = new();
        
        private static bool _dataLoaded;
        private SaveSettings _saveSettings;
        
        public async Task InitializeAsync(SaveSettings saveSettings, CancellationToken cancellationToken)
        {
            _saveSettings = saveSettings;
            
            try
            {
                Debug.Log($"<color=#99ff99>Initialize {nameof(LocalizationController)}.</color>");
                var timer = new SmallTimer();

                _dataLoaded = false;
                AvailableLanguagePacks = await LocalizationData.LoadAsync(cancellationToken);
                _dataLoaded = true;
          
                ChangeLocalization();
                Debug.Log($"<color=#99ff99>Time initialize {nameof(LocalizationController)}: {timer.Update()}.</color>");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
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

        
        public IEnumerable<SystemLanguage> Languages
        {
            get
            {
                return AvailableLanguagePacks.Select(i => i.Key);
            }
        }

        public SystemLanguage ActiveLanguage
        {
            get => _saveSettings.ActiveLanguage;
            set 
            {
                if (_saveSettings.ActiveLanguage != value)
                {
                    _saveSettings.ActiveLanguageDetected = true;
                    _saveSettings.ActiveLanguage = HasLanguage(value) ? value : DefaultLanguage;
                    ChangeLocalization();
                }
            }
        }

        public IEnumerable<LanguagePackDesc> LanguagePackDescs => AvailableLanguagePacks
            .Select(i => new LanguagePackDesc(i.Key, i.Value.NativeName, i.Value.NativeFlag));
        public bool ActiveLanguageDetected => _saveSettings.ActiveLanguageDetected;

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
        
        public LanguagePackDesc GetPackDesc(SystemLanguage language)
        {
            var foundPairIndex = AvailableLanguagePacks.FindIndex(i => i.Key == language);
            if (foundPairIndex >= 0)
            {
                var languagePack = AvailableLanguagePacks[foundPairIndex].Value;
                return new LanguagePackDesc(language, languagePack.NativeName, languagePack.NativeFlag);
            }

            return new LanguagePackDesc(language, language.ToString(), null);
        }
        
        public LanguagePackDesc GetActivePackDesc()
        {
            return GetPackDesc(_saveSettings.ActiveLanguage);
        }
        
        public static LanguagePackDesc GetPackDesc(int i)
        {
            var languagePack = AvailableLanguagePacks[i];

            return new LanguagePackDesc(languagePack.Key, languagePack.Value.NativeName, languagePack.Value.NativeFlag);
        }
        
        public string GetText(Atom.GuidEx guid)
        {
            var packIndex = GetPackIndex(ActiveLanguage);
            if (packIndex < 0) 
                return $"{LanguagePackNotFoundValue}({guid})";

            var text = AvailableLanguagePacks[packIndex].Value.Asset.FindText(guid);

            var s = text;
            if (s != null)
            {
                s = s.ToUpper();
                return s;
            }

            return $"{TextNotFoundValue}({guid})";
        }
        
        public static int LanguagesCountInEditorMode
        {
            get
            {
                if (!_dataLoaded)
                {
                    AvailableLanguagePacks = LocalizationData.Load();
                    _dataLoaded = true;
                }
                
                return AvailableLanguagePacks.Count;
            }
        }

       
        public static string GetTextInEditorMode(SystemLanguage language, Atom.GuidEx guid)
        {
            if (!_dataLoaded)
            {
                AvailableLanguagePacks = LocalizationData.Load();
                _dataLoaded = true;
            }
            
            var packIndex = GetPackIndex(language);
            if (packIndex < 0) 
                return $"{LanguagePackNotFoundValue}({guid})";

            var text = AvailableLanguagePacks[packIndex].Value.Asset.FindText(guid);
            if (text != null)
            {
                text = text.ToUpper();
            }
            
            return text ?? $"{TextNotFoundValue}({guid})";
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