// using System;
// using System.Collections.Generic;
// using Assets.Scripts.Common.Shared;
// using Atom;
// using UnityEngine;
// using UnityEngine.Serialization;
// using System.Threading.Tasks;
// using Assets.Scripts.Core.Storage;
// using Atom.Timers;
//
// namespace Assets.Scripts.Core.Localization
// {
//     public interface ILanguage
//     {
//         SystemLanguage Language { get; set; }
//     }
//
//     public interface ILocalizationSupport
//     {
//         void ChangeLocalization();
//     }
//
//     [Serializable]
//     public class LocalizationItem
//     {
//         public SystemLanguage Language;
//         public string NativeName;
//         public Sprite NativeFlag;  
//         public TextAsset Asset;
//     }
//     
//     [CreateAssetMenu(fileName = nameof(LocalizationManager), menuName = "Custom/" + nameof(LocalizationManager), order = 51)]
//     public class LocalizationManager : ScriptableObject
//     {
//         public const string TextNotFoundValue = "Text not found";
//         private const SystemLanguage mDefaultLanguage = SystemLanguage.English;
//
//         private static LocalizationManager mInstance;
//
//         private static readonly List<ILocalizationSupport> mLocalizationSupportObjects = new();
//         private static readonly List<KeyValuePair<SystemLanguage, LanguagePack>> mAvailableLanguagePacks = new();
//
//         [FormerlySerializedAs("Database")]
//         public List<LocalizationItem> Packs;
//
//         private static ILanguage mStorage;
//
//         private static SystemLanguage mLanguage = mDefaultLanguage;
//
//         public static async Task InitializeAsync(UserSettingsInfo storage)
//         {
//             var timer = new SmallTimer();
//
//             mStorage = storage;
//
//             await storage.GetInitializeTask();
//             //Debug.Log($"<color=#339933>Wait {nameof(LocalizationManager)}: {Conversion.ToString((float)timer.Update(), 2)}.</color>");
//
//             await InitializationAsync();
//
//             var lang = Application.systemLanguage;
//
//             if (storage.Language == SystemLanguage.Unknown)
//                 mLanguage = HasLanguage(lang) ? lang : mDefaultLanguage;
//             else
//                 mLanguage = HasLanguage(storage.Language) ? storage.Language : mDefaultLanguage;
//
//             mStorage.Language = mLanguage;
//
//             ChangeLocalization();
//
//             Debug.Log($"<color=#99ff99>Time initialize {nameof(LocalizationManager)}: {Conversion.ToString((float)timer.Update(), 2)}.</color>");
//         }
//
//         private static void Initialization()
//         {
//             if (mInstance == null)
//                 Reload();
//         }
//
//         public static void Reload()
//         {
//             var obj = Resources.Load(nameof(LocalizationManager));
//
//             if (obj == null)
//                 throw new Exception($"Resource '{nameof(LocalizationManager)}' not created.");
//
//             mInstance = obj as LocalizationManager;
//             if (mInstance == null)
//                 throw new Exception($"Resource '{nameof(LocalizationManager)}'  is not '{nameof(LocalizationManager)}'.");
//
//             mAvailableLanguagePacks.Clear();
//
//             foreach (var item in mInstance.Packs)
//             {
//                 mAvailableLanguagePacks.Add(new KeyValuePair<SystemLanguage, LanguagePack>(
//                     item.Language, 
//                     new LanguagePack(item.NativeName, item.NativeFlag, item.Asset)));
//             }    
//         }
//
//         public static async Task InitializationAsync()
//         {
//             var request = Resources.LoadAsync(nameof(LocalizationManager));
//
//             while (request.isDone)
//                 await Task.Yield();
//
//             if (request.asset == null)
//                 throw new Exception($"Resource '{nameof(LocalizationManager)}' not created.");
//
//             mInstance = request.asset as LocalizationManager;
//
//             if (mInstance == null)
//                 throw new Exception($"Resource '{nameof(LocalizationManager)}' is not '{nameof(LocalizationManager)}'.");
//
//             mAvailableLanguagePacks.Clear();
//
//             foreach (var item in mInstance.Packs)
//             {
//                 mAvailableLanguagePacks.Add(new KeyValuePair<SystemLanguage, LanguagePack>(
//                     item.Language,
//                     new LanguagePack(item.NativeName, item.NativeFlag, item.Asset)));
//             }
//         }
//
//         public static void Add(ILocalizationSupport listener)
//         {
//             mLocalizationSupportObjects.Add(listener);
//
//             try
//             {
//                 listener.ChangeLocalization();
//             }
//             catch (Exception e)
//             {
//                 SharedUtilities.LogException(e);
//             }
//         }
//         
//         public static void Remove(ILocalizationSupport listener)
//         {
//             mLocalizationSupportObjects.Remove(listener);
//         }
//
//         public static SystemLanguage GetLanguage()
//         {
//             return mLanguage;
//         }
//
//         public static void SetLanguage(SystemLanguage language)
//         {
//             if (mLanguage == language)
//                 return;
//
//             mLanguage = HasLanguage(language) ? language : mDefaultLanguage;
//             mStorage.Language = mLanguage;
//             ChangeLocalization();
//         }
//
//         private static void ChangeLocalization()
//         {
//             for (var i = 0; i != mLocalizationSupportObjects.Count; i++)
//             {
//                 var localizationSupportObject = mLocalizationSupportObjects[i];
//
//                 try
//                 {
//                     localizationSupportObject.ChangeLocalization();
//                 }
//                 catch (Exception e)
//                 {
//                     SharedUtilities.LogException(e);
//                 }
//             }
//         }
//
//         public static int LanguagesCount => mAvailableLanguagePacks.Count;
//
//         public static bool HasLanguage(SystemLanguage language)
//         {
//             return GetPackIndex(language) >= 0;
//         }
//
//         public static int GetPackIndex(SystemLanguage language)
//         {
//             return mAvailableLanguagePacks.FindIndex(i => i.Key == language);
//         }
//
//         public static LanguagePackDesc GetPackDesc(int i)
//         {
//             var languagePack = mAvailableLanguagePacks[i];
//
//             return new LanguagePackDesc(languagePack.Key, languagePack.Value.NativeName, languagePack.Value.NativeFlag);
//         }
//
//         public static string GetText(SystemLanguage language, Guid guid)
//         {
//             return GetText(language, Conversion.ToGuidEx(guid));
//         }
//
//         public static string GetText(SystemLanguage language, Atom.GuidEx guid)
//         {
//             Initialization();
//
//             var index = GetPackIndex(language);
//             
//             if (index < 0) 
//                 return $"{TextNotFoundValue}({guid})";
//
//             var text = mAvailableLanguagePacks[index].Value.Asset.FindText(guid);
//
//             return text ?? $"{TextNotFoundValue}({guid})";
//         }
//
//         public static bool HasText(GuidEx guid)
//         {
//             var index = GetPackIndex(mLanguage);
//             if (index >= 0)
//                 return mAvailableLanguagePacks[index].Value.Asset.FindText(guid) != null;
//
//             return false;           
//         }
//
//         public static string GetText(Guid guid)
//         {     
//             return GetText(mLanguage, Conversion.ToGuidEx(guid));
//         }
//
//         public static string GetText(Atom.GuidEx guid)
//         {
//             return GetText(mLanguage, guid);
//         }
//
//         public readonly struct LanguagePackDesc
//         {
//             public readonly SystemLanguage Language;
//             public readonly string NativeName;
//             public readonly Sprite NativeFlag;
//
//             public LanguagePackDesc(SystemLanguage packKey, string nativeName, Sprite nativeFlag)
//             {
//                 Language = packKey;
//                 NativeName = nativeName;
//                 NativeFlag = nativeFlag;
//             }
//         }
//
//         public class LanguagePack
//         {
//             public readonly string NativeName;
//             public readonly Sprite NativeFlag;
//             public Atom.Localization.LcText Asset;
//
//             public LanguagePack(string nativeName, Sprite nativeFlag, TextAsset data)
//             {
//                 NativeName = nativeName;
//                 NativeFlag = nativeFlag;
//                 Asset = new Atom.Localization.LcText(data.bytes);
//             }
//         }
//     }
// }