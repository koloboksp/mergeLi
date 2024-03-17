using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

    public int Score => _session.Score;

    public SessionCastleProgress Castle => _session.Castle;
    
    public SessionFieldProgress Field => _session.Field;

    public IReadOnlyList<SessionBuffProgress> Buffs => _session.Buffs;

    public void ChangeProgress(ISessionProgressHolder sessionProgressHolder)
    {
        var sessionProgress = new SessionProgress();

        sessionProgress.Score = sessionProgressHolder.GetScore();
        
        var castle = sessionProgressHolder.GetCastle();
        if (castle.Completed)
        {
            sessionProgress.Castle = new SessionCastleProgress()
            {
                Id = sessionProgressHolder.GetFirstUncompletedCastle(),
                Points = 0,
            };
        }
        else
        {
            sessionProgress.Castle = new SessionCastleProgress()
            {
                Id = castle.Id,
                Points = castle.GetPoints(),
            };
        }
        
        sessionProgress.Field = new SessionFieldProgress();
        foreach (var ball in sessionProgressHolder.GetField().GetAll<IBall>())
        {
            var ballProgress = new SessionBallProgress()
            {
                GridPosition = ball.IntGridPosition,
                Points = ball.Points,
            };
            sessionProgress.Field.Balls.Add(ballProgress);
        }

        sessionProgress.Buffs = new List<SessionBuffProgress>();
        foreach (var buff in sessionProgressHolder.GetBuffs())
        {
            var buffProgress = new SessionBuffProgress
            {
                Id = buff.Id,
                RestCooldown = buff.GetRestCooldown()
            };
            sessionProgress.Buffs.Add(buffProgress);
        }

        _controller.Save(_session, _fileName);
    }

    public bool IsValid()
    {
        return _session.IsValid();
    }
}