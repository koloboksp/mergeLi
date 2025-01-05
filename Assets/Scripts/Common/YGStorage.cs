using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Save;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public List<StringsPair> Data = new();
    }

    [Serializable]
    public class StringsPair
    {
        public string Key;
        public string Value;

        public StringsPair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}

namespace Assets.Scripts.Core.Storage
{
    public class YGStorage : IStorage
    {
        public void Save(string fullPath, string sData)
        {
            var pairI = YG2.saves.Data.FindIndex(i => i.Key == fullPath);
            if (pairI < 0)
            {
                YG2.saves.Data.Add(new StringsPair(fullPath, sData));
            }
            else
            {
                YG2.saves.Data[pairI] = new StringsPair(fullPath, sData);
            }
            YG2.SaveProgress();
        }

        public Task SaveAsync(string fullPath, string sData)
        {
            var pairI = YG2.saves.Data.FindIndex(i => i.Key == fullPath);
            if (pairI < 0)
            {
                YG2.saves.Data.Add(new StringsPair(fullPath, sData));
            }
            else
            {
                YG2.saves.Data[pairI] = new StringsPair(fullPath, sData);
            }
            YG2.SaveProgress();
            
            return Task.CompletedTask;
        }

        private TaskCompletionSource<List<StringsPair>> _loadingDataCompletionSource;
      
        public Task<string> Load(string fullPath)
        {
            return LoadInternal(fullPath);
        }

        public async Task<string> LoadInternal(string fullPath)
        {
            var result = await _loadingDataCompletionSource.Task;

            var pairI = result.FindIndex(i => i.Key == fullPath);
            if (pairI >= 0)
            {
                return result[pairI].Value;
            }
            
            return null;
        }

        public Task InitializeAsync()
        {
            _loadingDataCompletionSource = new TaskCompletionSource<List<StringsPair>>();

            if (!YG2.isSDKEnabled)
            {
                YG2.onGetSDKData += GetData;
            }
            else
            {
                GetData();
            }

            return Task.CompletedTask;
        }

        private void GetData()
        {
            _loadingDataCompletionSource.TrySetResult(YG2.saves.Data);
        }
    }
}