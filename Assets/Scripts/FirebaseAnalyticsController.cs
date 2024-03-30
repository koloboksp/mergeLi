using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace Core
{
    public class FirebaseAnalyticsController : IAnalyticsController
    {
        private const string VERSION = "VERSION";
        private const string TUTORIAL_STEP_COMPLETE = "TUTORIAL_STEP_COMPLETE";
        private const string TUTORIAL_STEP_NAME = "TUTORIAL_STEP_NAME";
        
        private FirebaseApp _firebase;
        private Atom.Version _version;
        
        public async Task InitializeAsync(Atom.Version version)
        {
            _version = version;
            
            var fixDependencies = FirebaseApp.CheckAndFixDependenciesAsync();
            await fixDependencies;
            
            if (fixDependencies.Result == DependencyStatus.Available)
            {
                _firebase = FirebaseApp.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Firebase not initialized: '{fixDependencies.Result}'");
            }
        }
        
        public void TutorialStepCompleted(string stepName)
        {
            if (_firebase == null)
                return;

            try
            {
                FirebaseAnalytics.LogEvent(
                    TUTORIAL_STEP_COMPLETE,
                    new Parameter(VERSION, _version.ToString()),
                    new Parameter(TUTORIAL_STEP_NAME, stepName));

                Debug.Log($"<color=Green>Analytics:</color> {TUTORIAL_STEP_COMPLETE} {stepName}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}