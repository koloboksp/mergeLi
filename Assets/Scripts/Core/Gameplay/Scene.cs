using System.Collections.Generic;
using System.Linq;
using Skins;
using UnityEngine;

namespace Core.Gameplay
{
    public interface IScene
    {
        SkinContainer ActiveSkin { get; }
        GameProcessor GameProcessor { get; }
        HatsLibrary HatsLibrary { get; }
        string[] ActiveHats { get; }
        public bool IsHatActive(string hatName);
    }

    public class Scene : MonoBehaviour, IScene, IHatsChanger
    {
        [SerializeField] private SkinsLibrary _skinsLibrary;
        [SerializeField] private HatsLibrary _hatsLibrary;
        [SerializeField] private GameProcessor _gameProcessor;
        [SerializeField] private Field _field;
        [SerializeField] private Transform _sceneRoot;

        private readonly List<string> _userActiveHatsFilter = new();
        private SkinContainer _activeSkin;
    
        public GameProcessor GameProcessor => _gameProcessor;
        public IField Field => _field;
        public Transform SceneRoot => _sceneRoot;
    
        public SkinsLibrary SkinLibrary => _skinsLibrary;
        public SkinContainer ActiveSkin => _activeSkin;
        public HatsLibrary HatsLibrary => _hatsLibrary;

        public void SetData(string skinName, string[] userInactiveHatsFilter)
        {
            ChangeUserActiveHatsFilter(userInactiveHatsFilter);
            SetSkin(skinName);
        }
   
        public void SetSkin(string skinName)
        {
            _activeSkin = _skinsLibrary.GetContainer(skinName);
            if (_activeSkin == null)
                _activeSkin = _skinsLibrary.GetDefaultSkin();
        
            var skinChangeables = _field.gameObject.GetComponentsInChildren<ISkinChangeable>();
            foreach (var skinChangeable in skinChangeables)
                skinChangeable.ChangeSkin(_activeSkin);
        }
    
        public string[] ActiveHats
        {
            get
            {
                var availableHats = _hatsLibrary.Hats;
            
                var activeHatIndexes = new List<string>();
                for (var hatI = 0; hatI < availableHats.Count; hatI++)
                {
                    var hat = availableHats[hatI];
                    if (!hat.Available)
                        continue;
                    if (_userActiveHatsFilter == null || !_userActiveHatsFilter.Contains(hat.Id))
                        continue;
                
                    activeHatIndexes.Add(hat.Id);
                }

                return activeHatIndexes.ToArray();
            }
        }
    
        public bool IsHatActive(string hatName)
        {
            Hat foundHat = null;
            foreach (var hat in _hatsLibrary.Hats)
            {
                if (hat.Id == hatName)
                    foundHat = hat;
            }
        
            if (foundHat == null) 
                return false;
            if (!foundHat.Available)
                return false;
            if (_userActiveHatsFilter == null || !_userActiveHatsFilter.Contains(foundHat.Id))
                return false;

            return true;
        }

        public void SetUserActiveHatsFilter(string[] userActiveHatsFilter)
        {
            ChangeUserActiveHatsFilter(userActiveHatsFilter);

            ApplicationController.Instance.SaveController.SaveSettings.UserActiveHatsFilter = userActiveHatsFilter;
        }

        private void ChangeUserActiveHatsFilter(string[] userActiveHatsFilter)
        {
            _userActiveHatsFilter.Clear();
            if (userActiveHatsFilter != null)
                _userActiveHatsFilter.AddRange(userActiveHatsFilter);
        
            var hatChangeables = _field.gameObject.GetComponentsInChildren<IHatChangeable>();
            foreach (var hatChangeable in hatChangeables)
                hatChangeable.ChangeUserActiveHatsFilter();

            var hatsExtraPoints = _hatsLibrary.Hats
                .Where(hat => _userActiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) != null)
                .Select(hat => (hat.Id, hat.ExtraPoints));
        
            _gameProcessor.PointsCalculator.UpdateHatsExtraPoints(hatsExtraPoints);
        }

        public string[] GetUserActiveHatsFilter()
        {
            return ApplicationController.Instance.SaveController.SaveSettings.UserActiveHatsFilter;
        }
    }

    public interface ISkinChanger
    {
        public void SetSkin(string skinName);
    }

    public interface IHatsChanger
    {
        //  public void SetHat(string hatName);
        public void SetUserActiveHatsFilter(string[] userActiveHatsFilter);
        public string[] GetUserActiveHatsFilter();
    }
}