using System;
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
            Downgrade = 32
        }

        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private Transform _hatAnchor;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private AudioClip _onSelectClip;
        [SerializeField] private AudioClip _onMoveClip;
        [SerializeField] private AudioClip _onUpgradeClip;
        [SerializeField] private AudioClip _onDowngradeClip;
        [SerializeField] private AudioClip _onPathNotFoundClip;

        [SerializeField] private DestroyBallEffect _destroyEffectPrefab;

        private BallView _view;
        private HatView _hatView;
        private DependencyHolder<SoundsPlayer> _soundsPlayer;

        public UnityAction<BallState> ChangeStateEvent;

        public override BallView View
        {
            set => _view = value;
        }

        public override bool Selected
        {
            set
            {
                ChangeStateEvent?.Invoke(value ? BallState.Select : BallState.Idle);
                _hatAnchor.gameObject.SetActive(value);
                
                if (value)
                {
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
                    _soundsPlayer.Value.StartPlay(_onMoveClip);
                }
                else
                {
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
                _soundsPlayer.Value.Play(_onUpgradeClip);
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
        }

        public override void SetHat(HatView hatView)
        {
            if (_hatView != null)
            {
                Destroy(_hatView.gameObject);
                _hatView = null;
            }

            if (hatView != null)
            {
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
    }
}