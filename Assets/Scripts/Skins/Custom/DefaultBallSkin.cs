using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Effects;
using Core.Gameplay;
using Core.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Skins.Custom
{
    [Serializable]
    public class ShapeToPointsAssociation
    {
        [SerializeField] private int _points;
        [SerializeField] private Sprite _shape;

        public int Points => _points;
        public Sprite Shape => _shape;
    }
    
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
            Born = 1 << 7,
        }

        [SerializeField] private Text _valueLabel;
        [SerializeField] private Image _ballIcon;
        [SerializeField] private Transform _hatAnchor;
        [SerializeField] private Transform _faceAnchor;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private AudioClip[] _onSelectClips;
        [SerializeField] private AudioClip _onMoveClip;
        [SerializeField] private AudioClip[] _onUpgradeClips;
        [SerializeField] private AudioClip[] _onDowngradeClips;
        [SerializeField] private AudioClip _onPathNotFoundClip;

        [SerializeField] private Image _shapeIcon;
        [SerializeField] private List<ShapeToPointsAssociation> _shapeAssociations;
        [SerializeField] private BlobFace _facePrefab;
        [SerializeField] private DestroyBallEffect _destroyEffectPrefab;

        private BallView _view;
        private BlobFace _face;
        private HatView _hatView;
        private DependencyHolder<SoundsPlayer> _soundsPlayer;
        private Coroutine _hideFaceWithDelay;
        
        public UnityAction<BallState, bool> ChangeStateEvent;

        public override void SetData(BallView view)
        {
            _view = view;
            _face = Instantiate(_facePrefab, _faceAnchor);
        }

        public override void Born()
        {
            ChangeStateEvent?.Invoke(BallState.Born, true);
        }
        
        public override bool Selected
        {
            set
            {
                ChangeStateEvent?.Invoke(value ? BallState.Select : BallState.Idle, false);
                _faceAnchor.gameObject.SetActive(value);
                
                if (value)
                {
                    BreakHideFaceWithDelay();
                    _face.ShowLocal(BallState.Select);
                    
                    if (_onSelectClips.Length > 0)
                        _soundsPlayer.Value.Play(_onSelectClips[UnityEngine.Random.Range(0,_onSelectClips.Length)]);
                }
            }
        }

        public override bool Moving
        {
            set
            {
                ChangeStateEvent?.Invoke(value ? BallState.Move : BallState.Idle, false);
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
            SetShape(points);
            
            if (force)
                return;
            
            var ballState = points >= oldPoints ? BallState.Upgrade : BallState.Downgrade;

            ChangeStateEvent?.Invoke(ballState, false);

            if (ballState == BallState.Upgrade)
            {
                if (_onUpgradeClips.Length > 0)
                    _soundsPlayer.Value.Play(_onUpgradeClips[UnityEngine.Random.Range(0, _onUpgradeClips.Length)]);
                
                _faceAnchor.gameObject.SetActive(true);
                _face.ShowLocal(BallState.Upgrade);
                HideFaceWithDelay(1.0f);
            }

            if (ballState == BallState.Downgrade)
            {
                if(_onDowngradeClips.Length > 0)
                    _soundsPlayer.Value.Play(_onDowngradeClips[UnityEngine.Random.Range(0, _onDowngradeClips.Length)]);
            }
        }

        private void SetShape(int points)
        {
            var foundAssociation = _shapeAssociations.Find(i => i.Points == points);
            if (foundAssociation != null)
                _shapeIcon.sprite = foundAssociation.Shape;
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
            ChangeStateEvent?.Invoke(BallState.PathNotFound, false);
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
                
            destroyBallEffect.Run(_view.MainColor);
        }

        public override void ShowHat(bool activeState)
        {
            _hatAnchor.gameObject.SetActive(activeState);
        }

        public override void SetHat(string hatName, string oldHat, bool force)
        {
            Hat hat = null;
            if (!string.IsNullOrEmpty(_view.Ball.HatName))
            {
                if (_view.Ball.Field.Scene.IsHatActive(hatName))
                {
                    hat = _view.Ball.Field.Scene.HatsLibrary.Hats.FirstOrDefault(i => i.Id == hatName);
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

        public override void ChangeUserActiveHatsFilter()
        {
            SetHat(_view.Ball.HatName, _view.Ball.HatName, true);
        }

        public override void ShowPoints(bool activeState)
        {
            _valueLabel.gameObject.SetActive(activeState);
        }

        public override void ShowEyes(bool activeState)
        {
            _faceAnchor.gameObject.SetActive(activeState);
            if (activeState)
            {
                _face.ShowLocal(BallState.Select);
            }
        }

        

        private IEnumerator HideFaceWithDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeStateEvent?.Invoke(BallState.AutoDeselect, false);
            
            yield return new WaitForSeconds(0.5f);
            ChangeStateEvent?.Invoke(BallState.Idle, false);
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