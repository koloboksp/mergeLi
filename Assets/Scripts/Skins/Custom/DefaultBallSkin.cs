using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Effects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Skins.Custom
{
    public class DefaultBallSkin : BallSkin
    {
        [Flags]
        public enum BallState
        {
            None = 0,
            Idle = 1,
            Select = 2,
            Move = 4,
            PathNotFound = 8,
            Upgrade = 16,
            Downgrade = 32,
            AutoDeselect = 1 << 6,
        }

        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private Transform _hatAnchor;
        [SerializeField] private Transform _faceAnchor;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private AudioClip _onSelectClip;
        [SerializeField] private AudioClip _onMoveClip;
        [SerializeField] private AudioClip _onUpgradeClip;
        [SerializeField] private AudioClip _onDowngradeClip;
        [SerializeField] private AudioClip _onPathNotFoundClip;

        [SerializeField] private BlobFace _facePrefab;
        [SerializeField] private DestroyBallEffect _destroyEffectPrefab;

        private BallView _view;
        private BlobFace _face;
        private HatView _hatView;
        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        private Coroutine _hideFaceWithDelay;
        
        public UnityAction<BallState> ChangeStateEvent;

        public override void SetData(BallView view)
        {
            _view = view;
            _face = Instantiate(_facePrefab, _faceAnchor);
            
           
        }

        public override bool Selected
        {
            set
            {
                ChangeStateEvent?.Invoke(value ? BallState.Select : BallState.Idle);
                _faceAnchor.gameObject.SetActive(value);
                
                if (value)
                {
                    BreakHideFaceWithDelay();
                    _face.ShowLocal(BallState.Select);
                    _soundsPlayer.Value.Play(_onSelectClip);
                }
            }
        }

        public override bool Moving
        {
            set
            {
                ChangeStateEvent?.Invoke(value ? BallState.Move : BallState.Idle);
                if (value)
                {
                    _faceAnchor.gameObject.SetActive(true);
                    _face.ShowLocal(BallState.Move);
                    
                    _soundsPlayer.Value.StartPlay(_onMoveClip);
                }
                else
                {
                    _faceAnchor.gameObject.SetActive(true);
                    _face.ShowLocal(BallState.Idle);
                    HideFaceWithDelay(0.1f);
                    _soundsPlayer.Value.StopPlay();
                }
            }
        }
        
        public override void SetPoints(int points, int oldPoints, bool force)
        {
            _valueLabel.text = points.ToString();

            if (force)
                return;
            
            var ballState = points >= oldPoints ? BallState.Upgrade : BallState.Downgrade;

            ChangeStateEvent?.Invoke(ballState);

            if (ballState == BallState.Upgrade)
            {
                _soundsPlayer.Value.Play(_onUpgradeClip);
                
                _faceAnchor.gameObject.SetActive(true);
                _face.ShowLocal(BallState.Upgrade);
                HideFaceWithDelay(1.0f);
            }

            if (ballState == BallState.Downgrade)
                _soundsPlayer.Value.Play(_onDowngradeClip);
        }

        public override Color MainColor
        {
            set => _ballIcon.color = value;
        }

        public override float Transparency
        {
            set => _canvasGroup.alpha = 1.0f - value;
        }

        public override void PathNotFount()
        {
            ChangeStateEvent?.Invoke(BallState.PathNotFound);
            _soundsPlayer.Value.Play(_onPathNotFoundClip);
            
            _faceAnchor.gameObject.SetActive(true);
            _face.ShowLocal(BallState.PathNotFound);
        }

        private void ChangeHat(HatView hatView)
        {
            if (_hatView != null)
            {
                Destroy(_hatView.gameObject);
                _hatView = null;
            }

            if (hatView != null)
            {
                _hatAnchor.gameObject.SetActive(true);
                _hatView = Instantiate(hatView, _hatAnchor);
            }
        }

        public override void Remove(bool force)
        {
            if (force)
                return;
            
            var destroyBallEffect = Object.Instantiate(
                _destroyEffectPrefab, 
                transform.position, 
                Quaternion.identity, 
                _view.Ball.Field.View.Root);
                
            destroyBallEffect.Run(_view.Ball.GetColorIndex());
        }

        public override void ShowHat(bool activeState)
        {
            _hatAnchor.gameObject.SetActive(activeState);
        }

        public override void SetHat(int hatI, int oldHatI, bool force)
        {
            Hat hat = null;
            if (_view.Ball.Hat != 0)
            {
                if (_view.Ball.Field.Scene.IsHatActive(hatI))
                {
                    var hats = _view.Ball.Field.Scene.HatsLibrary.Hats;
                    if (hatI < hats.Count)
                        hat = hats[hatI];
                }
            }

            if (hat != null)
            {
                _hatAnchor.gameObject.SetActive(true);
                ChangeHat(hat.View);
            }
            else
            {
                _hatAnchor.gameObject.SetActive(false);
                ChangeHat(null);
            }
        }

        public override void ChangeUserInactiveHatsFilter()
        {
            SetHat(_view.Ball.Hat, _view.Ball.Hat, true);
        }

        private IEnumerator HideFaceWithDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeStateEvent?.Invoke(BallState.AutoDeselect);
            
            yield return new WaitForSeconds(0.5f);
            ChangeStateEvent?.Invoke(BallState.Idle);
            _faceAnchor.gameObject.SetActive(false);
        }

        private void BreakHideFaceWithDelay()
        {
            if (_hideFaceWithDelay != null)
            {
                StopCoroutine(_hideFaceWithDelay);
                _hideFaceWithDelay = null;
            }
        }
        
        private void HideFaceWithDelay(float delay)
        {
            BreakHideFaceWithDelay();
            _hideFaceWithDelay = StartCoroutine(HideFaceWithDelayCoroutine(delay));
        }
    }
}