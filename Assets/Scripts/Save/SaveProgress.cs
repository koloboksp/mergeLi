using System;
using System.Threading;
using System.Threading.Tasks;

namespace Save
{
    public class SaveProgress
    {
        private readonly SaveController _controller;
        private readonly string _fileName;

        private Progress _progress = new Progress();

        public SaveProgress(SaveController controller, string fileName)
        {
            _controller = controller;
            _fileName = fileName;
        }

        public void AddCoins(int amount)
        {
            _progress.Coins += amount;
            _controller.Save(_progress, _fileName);
        }

        public int GetAvailableCoins()
        {
            return _progress.Coins;
        }

        public void ConsumeCoins(int count)
        {
            _progress.Coins -= count;
            _controller.Save(_progress, _fileName);
        }

        public bool IsCastleCompleted(string id)
        {
            return _progress.CompletedCastles.Contains(id);
        }

        public void MarkCastleCompleted(string id)
        {
            if (_progress.CompletedCastles.Contains(id))
                return;

            _progress.CompletedCastles.Add(id);
            _controller.Save(_progress, _fileName);
        }
        
       

        public void SetBestSessionScore(int score)
        {
            _progress.BestSessionScore = score;
            _controller.Save(_progress, _fileName);
        }

        public int BestSessionScore => _progress.BestSessionScore;

        public bool IsTutorialComplete(string tutorialId)
        {
            var tutorialProgress = _progress.Tutorials.Find(i => string.Equals(i.Id, tutorialId, StringComparison.Ordinal));
            if (tutorialProgress != null)
                return tutorialProgress.IsComplted;

            return false;
        }

        public void CompleteTutorial(string tutorialId)
        {
            var tutorialProgress = _progress.Tutorials.Find(i => string.Equals(i.Id, tutorialId, StringComparison.Ordinal));
            if (tutorialProgress != null)
                tutorialProgress.IsComplted = true;
            else
            {
                _progress.Tutorials.Add(
                    new TutorialProgress()
                    {
                        Id = tutorialId,
                        IsComplted = true,
                    });
            }

            _controller.Save(_progress, _fileName);
        }

        public long GetGiftLastCollectedTimestamp(string id)
        {
            var giftProgress = _progress.Gifts.Find(i => i.Id == id);
            if (giftProgress != null)
                return giftProgress.LastCollectedTimestamp;

            return -1;
        }

        public async Task<bool> SetGiftLastCollectedTimestamp(string id, long lastCollectedTimestamp)
        {
            var giftProgress = _progress.Gifts.Find(i => i.Id == id);
            if (giftProgress == null)
            {
                giftProgress = new GiftProgress()
                {
                    Id = id,
                };
                _progress.Gifts.Add(giftProgress);
            }

            giftProgress.TimesCollected++;
            giftProgress.LastCollectedTimestamp = lastCollectedTimestamp;

            var saveResult = await _controller.SaveAsync(_progress, _fileName);

            return saveResult;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var loadedProgress = await _controller.LoadAsync<Progress>(_fileName, cancellationToken);
            if (loadedProgress != null)
                _progress = loadedProgress;
        }

        public void Clear()
        {
            _progress = new Progress();
            _controller.Clear(_fileName);
        }
#if DEBUG
        public void DebugChangeCastleComplete(string id, bool state)
        {
            if (state)
            {
                if (!_progress.CompletedCastles.Contains(id))
                {
                    _progress.CompletedCastles.Add(id);
                    _controller.Save(_progress, _fileName);
                }
            }
            else
            {
                if(_progress.CompletedCastles.Remove(id))
                    _controller.Save(_progress, _fileName);
            }
        }
        
        public void DebugChangeTutorialCompleteState(string tutorialId, bool state)
        {
            var tutorialProgress = _progress.Tutorials.Find(i => string.Equals(i.Id, tutorialId, StringComparison.Ordinal));

            if (state)
            {
                if (tutorialProgress != null)
                    tutorialProgress.IsComplted = true;
                else
                {
                    _progress.Tutorials.Add(
                        new TutorialProgress()
                        {
                            Id = tutorialId,
                            IsComplted = true,
                        });
                }
            }
            else
            {
                if (tutorialProgress != null)
                    _progress.Tutorials.Remove(tutorialProgress);
            }

            _controller.Save(_progress, _fileName);
        }
#endif
    }
}