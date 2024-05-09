using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Save
{
    public class SaveSessionProgress
    {
        private SessionProgress _session = new SessionProgress();

        private readonly SaveController _controller;
        private readonly string _fileName;

        public SaveSessionProgress(SaveController controller, string fileName)
        {
            _controller = controller;
            _fileName = fileName;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var loadedSession = await _controller.LoadAsync<SessionProgress>(_fileName, cancellationToken);
            if (loadedSession != null)
                _session = loadedSession;
        }
    
        public void Clear()
        {
            _session = new SessionProgress();
            _controller.Clear(_fileName);
        }
        
        public SessionCastleProgress ActiveCastle => _session.ActiveCastle;
        public IReadOnlyList<SessionCastleProgress> CompletedCastles => _session.CompletedCastles;

        public SessionFieldProgress Field => _session.Field;

        public IReadOnlyList<SessionBuffProgress> Buffs => _session.Buffs;

        public SessionAnalyticsProgress Analytics => _session.Analytics;

        public void ChangeProgress(ISessionProgressHolder sessionProgressHolder)
        {
            var completedCastles = sessionProgressHolder.GetCompletedCastles();
            _session.CompletedCastles.Clear();
            _session.CompletedCastles.AddRange(
                completedCastles.Select(i => new SessionCastleProgress()
                {
                    IsValid = true,
                    Id = i.Id,
                    Points = i.GetPoints(),
                }));

            var activeCastle = sessionProgressHolder.GetActiveCastle();
            _session.ActiveCastle = new SessionCastleProgress()
            {
                IsValid = true,
                Id = activeCastle.Id,
                Points = activeCastle.GetPoints(),
            };
            
            _session.Field = new SessionFieldProgress();
            foreach (var ball in sessionProgressHolder.GetField().GetAll<IBall>())
            {
                var ballProgress = new SessionBallProgress()
                {
                    GridPosition = ball.IntGridPosition,
                    Points = ball.Points,
                    HatHame = ball.HatName,
                };
                _session.Field.Balls.Add(ballProgress);
            }

            _session.Buffs = new List<SessionBuffProgress>();
            foreach (var buff in sessionProgressHolder.GetBuffs())
            {
                var buffProgress = new SessionBuffProgress
                {
                    Id = buff.Id,
                    RestCooldown = buff.GetRestCooldown()
                };
                _session.Buffs.Add(buffProgress);
            }

            var commonAnalytics = sessionProgressHolder.GetCommonAnalyticsAnalytics();
            _session.Analytics = new SessionAnalyticsProgress();
            _session.Analytics.Step = commonAnalytics.GetStep();
            _session.Analytics.StepsTakenInto = new List<StepTakenInto>();
            foreach (var stepTakenIntoInfo in commonAnalytics.GetStepsTakenIntoInfos())
            {
                _session.Analytics.StepsTakenInto.Add(new StepTakenInto()
                {
                    StepTag = stepTakenIntoInfo.Tag,
                    Count = stepTakenIntoInfo.Count,
                });   
            }
            _controller.Save(_session, _fileName);
        }

        public bool IsValid()
        {
            return _session.IsValid();
        }
    }
}