using System.Threading.Tasks;
using Atom;
using Core;
using Save;
using Skins;
using UnityEngine;

public class Hat : MonoBehaviour
{
    [SerializeField] private bool _isFree;
    [SerializeField] private int _groupIndex = 0;
    [SerializeField] private int _extraPoints = 1;
    [SerializeField] private int _cost;
    [SerializeField] private HatView _hatView;
    [SerializeField] private GuidEx _nameKey;
    
    private SaveProgress _saveProgress;
        
    public string Id => gameObject.name;
    public bool IsFree => _isFree;
    public int Cost => _cost;
    public HatView View => _hatView;
    public GuidEx NameKey => _nameKey;
    public int ExtraPoints => _extraPoints;
    public int GroupIndex => _groupIndex;

    public bool Available
    {
        get
        {
            if (_isFree)
                return true;

            if (_saveProgress.IsHatBought(Id))
                return true;

            return false;
        }
    }

    
    public void SetData(SaveProgress saveProgress)
    {
        _saveProgress = saveProgress;
    }

    public async Task Buy()
    {
        if (_saveProgress.GetAvailableCoins() >= _cost)
        {
            _saveProgress.ConsumeCoins(_cost);
            _saveProgress.BuyHat(Id);
        }
    }
}