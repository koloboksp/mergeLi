using System.Threading;
using System.Threading.Tasks;
using Save;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public string Data;
    }
}

namespace Assets.Scripts.Core.Storage
{
    public class YGStorage : IStorage
    {
        public void Save(string fullPath, string sData)
        {
            YG2.saves.Data = sData;
            YG2.SaveProgress();
        }

        public Task SaveAsync(string fullPath, string sData)
        {
            YG2.saves.Data = sData;
            YG2.SaveProgress();
            
            return Task.CompletedTask;
        }

        private TaskCompletionSource<string> _loadingCompletionSource;
        private CancellationTokenSource _loadingTimeoutCancellationTokenSource;

        public Task<string> Load(string fullPath)
        {
            _loadingCompletionSource = new TaskCompletionSource<string>();
            _loadingTimeoutCancellationTokenSource = new CancellationTokenSource();
            _loadingTimeoutCancellationTokenSource.CancelAfter(15000);
            var loadingTimeoutCancellationToken = _loadingTimeoutCancellationTokenSource.Token;
            var loadingTimeoutCancellationTokenRegistration = loadingTimeoutCancellationToken.Register(() => _loadingCompletionSource.TrySetResult(null));
            
            return _loadingCompletionSource.Task;
        }

        public Task InitializeAsync()
        {
            YG2.onGetSDKData += GetData;

            return Task.CompletedTask;
        }

        private void GetData()
        {
            _loadingTimeoutCancellationTokenSource.Dispose();
            _loadingCompletionSource.TrySetResult(YG2.saves.Data);
        }
    }
}