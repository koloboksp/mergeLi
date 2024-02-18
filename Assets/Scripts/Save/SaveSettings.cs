using System.Threading;
using System.Threading.Tasks;

public class SaveSettings
{
    private Settings _settings = new Settings();

    private readonly SaveController _controller;
    private readonly string _fileName; 
    
    public SaveSettings(SaveController controller, string fileName)
    {
        _controller = controller;
        _fileName = fileName;
    }

    public float SoundVolume
    {
        get => _settings.SoundVolume;
        set
        {
            if (value != _settings.SoundVolume)
            {
                _settings.SoundVolume = value;
                _controller.Save(_settings, _fileName);
            }
        }
        
    }
    
    public bool SoundEnable
    {
        get => _settings.SoundEnable;
        set
        {
            if (value != _settings.SoundEnable)
            {
                _settings.SoundEnable = value;
                _controller.Save(_settings, _fileName);
            }
        }
        
    }
    
    public float MusicVolume
    {
        get => _settings.MusicVolume;
        set
        {
            if (value != _settings.MusicVolume)
            {
                _settings.MusicVolume = value;
                _controller.Save(_settings, _fileName);
            }
        }
        
    }
    
    public bool MusicEnable
    {
        get => _settings.MusicEnable;
        set
        {
            if (value != _settings.MusicEnable)
            {
                _settings.MusicEnable = value;
                _controller.Save(_settings, _fileName);
            }
        }
        
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var loadedSettings = await _controller.LoadAsync<Settings>(_fileName, cancellationToken);
        if(loadedSettings != null)
            _settings = loadedSettings;
    }
}