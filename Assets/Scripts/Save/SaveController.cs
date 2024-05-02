using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core.Storage;
using UnityEngine;

namespace Save
{
    public class SaveController
    {
        const string PlayerSettingsFileName = "playerSettings";
        const string PlayerDataFileName = "playerData";
        const string PlayerLastSessionDataFileName = "playerLastSessionData";

        private BaseStorage _storage;
        
        private SessionProgress _lastSessionProgress = null;

        private readonly SaveSettings _saveSettings;
        private readonly SaveProgress _saveProgress;
        private readonly SaveSessionProgress _saveLastSessionProgress;

        public SaveSettings SaveSettings => _saveSettings;
        public SaveProgress SaveProgress => _saveProgress;
        public SaveSessionProgress SaveLastSessionProgress => _saveLastSessionProgress;

        public SaveController()
        {
            _saveSettings = new SaveSettings(this, PlayerSettingsFileName);
            _saveProgress = new SaveProgress(this, PlayerDataFileName);
            _saveLastSessionProgress = new SaveSessionProgress(this, PlayerLastSessionDataFileName);
        }

        public async Task InitializeAsync(BaseStorage storage, CancellationToken cancellationToken)
        {
            _storage = storage;
            try
            {
                await _saveSettings.InitializeAsync(cancellationToken);
                await _saveProgress.InitializeAsync(cancellationToken);
                await _saveLastSessionProgress.InitializeAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Clear()
        {
            try
            {
                _saveSettings.Clear();
                _saveProgress.Clear();
                _saveLastSessionProgress.Clear();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal bool Save<T>(T data, string fileName)
        {
            try
            {
                var sData = JsonUtility.ToJson(data);
                var fullPath = Path.Combine(Application.persistentDataPath, fileName);
                _storage.Save(fullPath, sData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }

        internal async Task<bool> SaveAsync<T>(T data, string fileName)
        {
            try
            {
                var sData = JsonUtility.ToJson(data);
                var fullPath = Path.Combine(Application.persistentDataPath, fileName);
                await _storage.SaveAsync(fullPath, sData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }

        internal void Clear(string fileName)
        {
            try
            {
                var fullPath = Path.Combine(Application.persistentDataPath, fileName);
                File.Delete(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal async Task<T> LoadAsync<T>(string fileName, CancellationToken cancellationToken) where T : class
        {
            try
            {
                var fullPath = Path.Combine(Application.persistentDataPath, fileName);
                string loadedData = _storage.Load(fullPath);
                return JsonUtility.FromJson<T>(loadedData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }
    }
}