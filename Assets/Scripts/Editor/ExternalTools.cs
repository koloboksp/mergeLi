using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Assets.Scripts.Editor
{
    public class ExternalTools : MonoBehaviour
    {
        private const int WaitTimeAfterStart = 1000;
        
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/LcImport")]
        static void StartLcImport()
        {
            var pathFolder = Path.GetFullPath(Application.dataPath + $"/../Tools/LcImport/");
            var pathExe = pathFolder + $"LcImport.exe";
            RunExe(pathFolder, pathExe);
        }
        //-----------------------------------------------------------------------------------------
        private static void RunExe(string pathFolder, string pathExe)
        {
            var p = new Process();
            var ps = new ProcessStartInfo(pathExe) { WorkingDirectory = pathFolder };
            p.StartInfo = ps;
            p.Start();
            p.WaitForExit(WaitTimeAfterStart);
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/LocalizationDocument")]
        static void OpenLocalizationDocument()
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1ksUJL61j-cxS012db2iDCuUL8wCNag_zlErZWZ7Ht38");
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/MainDocument")]
        static void OpenMainDocument()
        {
            Application.OpenURL("https://docs.google.com/spreadsheets/d/1SmEM07-HbXDgiOC8ZZHSquyO7gtQ23y8oNszo5C-Dm0");
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/Assembly")]
        static void OpenAssembly()
        {
            Application.OpenURL("https://drive.google.com/file/d/1KU__5Nv6tDr9U-4w5q4-XwZvk9ZxTAyY");
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/GuidGenerator")]
        static void OpenGuidGenerator()
        {
	        Application.OpenURL("https://www.guidgenerator.com");
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/ToPlayMarket")]
        static void OpenOnPlayMarket()
        {
            //Application.OpenURL(LFDefines.PlayMarketReference);
        }
        [MenuItem("Tools/OpenFirebaseConsole")]
        static void OpenFirebaseConsole()
        {
	       // Application.OpenURL(LFDefines.FireBaseReference);
        }
        //-----------------------------------------------------------------------------------------
        [MenuItem("Tools/OpenConfigsFolder")]
        static void OpenConfigsFolder()
        {
            var path = Application.persistentDataPath + Path.DirectorySeparatorChar;
            Process.Start(path);
        }
    }
}