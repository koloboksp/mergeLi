using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Core
{
    public class MusicPlayer : MonoBehaviour, ISoundControllerListener
    {
        const string MODIFICATOR_NAME = "Player";
        
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private SoundHolder _soundHolder;
        [SerializeField] private float _soundBlendTime = 3.0f;

        private CancellationTokenSource _stopPlaying;
        private CancellationTokenSource _stopPlayingCurrentClip;
        private Task _playingTask;
        private List<int> _clipsOrder = new List<int>();

        private void OnEnable()
        {
            SoundController.AddListener(this);
        }

        private void OnDisable()
        {
            SoundController.RemoveListener(this);
        }

        public void SetData(GameProcessor processor)
        {
            processor.OnRestart += Processor_OnRestart;
            processor.OnLose += Processor_OnLose;

            if (_playingTask == null)
            {
                _playingTask = Play(Application.exitCancellationToken);
            }
        }

        private void Stop()
        {
            if (_stopPlaying != null)
            {
                _stopPlaying.Cancel();
            }

            _playingTask = null;
        }

        private void Processor_OnLose()
        {
            PlayNext();
        }

        private void Processor_OnRestart()
        {
            PlayNext();
        }

        private void PlayNext()
        {
            if (_playingTask != null)
            {
                if (_stopPlayingCurrentClip != null)
                {
                    _stopPlayingCurrentClip.Cancel();
                }
            }
            else
            {
                _playingTask = Play(Application.exitCancellationToken);
            }
        }

        private bool _lastState;
        private float _lastTime;
        private async Task Play(CancellationToken exit)
        {
            _stopPlaying = new CancellationTokenSource();
            
            try
            {
                while (true)
                {
                    Application.exitCancellationToken.ThrowIfCancellationRequested();

                    if (_stopPlaying.IsCancellationRequested)
                        break;

                    _stopPlayingCurrentClip = new CancellationTokenSource();

                    ShakeIfRequired();

                    var audioClip = ConsumeClip();
                    var clipLength = audioClip.length;
                    var playTime = clipLength - _soundBlendTime * 2.0f;

                    _soundHolder.Clip = audioClip;
                    _soundHolder.Play();

                    await SoundBlend(_soundBlendTime, 1, exit, _stopPlaying.Token);
                    await WaitForPlaying(playTime, exit, _stopPlaying.Token, _stopPlayingCurrentClip.Token);
                    await SoundBlend(_soundBlendTime, -1, exit, _stopPlaying.Token);

                    _soundHolder.Stop();
                    
                    if (_stopPlayingCurrentClip != null)
                    {
                        _stopPlayingCurrentClip.Dispose();
                        _stopPlayingCurrentClip = null;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                if (_stopPlayingCurrentClip != null)
                {
                    _stopPlayingCurrentClip.Dispose();
                    _stopPlayingCurrentClip = null;
                }
                
                if (_stopPlaying != null)
                {
                    _stopPlaying.Dispose();
                    _stopPlaying = null;
                }
            }
            
            _soundHolder.SetVolumeModificator(MODIFICATOR_NAME, 0.0f);
        }

        private AudioClip ConsumeClip()
        {
            var audioClip = _clips[_clipsOrder[0]];
            _clipsOrder.RemoveAt(0);
            return audioClip;
        }

        private async Task WaitForPlaying(float playTime, 
            CancellationToken exit, 
            CancellationToken stopPlaying, 
            CancellationToken stopPlayingCurrentClip)
        {
            var timer = 0.0f;

            while (timer <= playTime)
            {
                exit.ThrowIfCancellationRequested();

                if (stopPlaying.IsCancellationRequested)
                    return;
                if (stopPlayingCurrentClip.IsCancellationRequested)
                    return;
                
                await Task.Yield();

                timer += Time.deltaTime;
            }
        }

        private async Task SoundBlend(
            float muteTime, 
            int direction,
            CancellationToken exit, 
            CancellationToken stopPlaying)
        {
            var timer = 0.0f;

            while (timer <= muteTime)
            {
                exit.ThrowIfCancellationRequested();

                if (stopPlaying.IsCancellationRequested)
                    return;

                var nTime = timer / muteTime;
                nTime = Mathf.Clamp01(nTime);
                if (direction < 0)
                    nTime = 1.0f - nTime;
                
                _soundHolder.SetVolumeModificator(MODIFICATOR_NAME, nTime);
                await Task.Yield();

                timer += Time.deltaTime;
            }
        }

        private void ShakeIfRequired()
        {
            if (_clipsOrder.Count <= 0)
            {
                var availableClips = new List<int>();
                for (var i = 0; i < _clips.Length; i++)
                    availableClips.Add(i);

                _clipsOrder = new List<int>();
                while (availableClips.Count > 0)
                {
                    var clipI = Random.Range(0, availableClips.Count - 1);
                    _clipsOrder.Add(availableClips[clipI]);
                    availableClips.RemoveAt(clipI);
                }
            }
        }


        public SoundGroup Group => SoundGroup.Music;

        public void ChangeVolume(float volume)
        {
            if (volume > 0)
            {
                if (_playingTask == null)
                {
                    _playingTask = Play(Application.exitCancellationToken);
                }
            }
            else
            {
                Stop();
            }
        }
    }
}