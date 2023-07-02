using System;
using Core;
using Core.Steps.CustomOperations;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private DefaultMarket _market;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;

    public event Action OnCoinsChanged;
    
    private int _coinsAmount = 10;

    private void Awake()
    {
        _market.OnBought += Market_OnBought;
    }

    private void Market_OnBought(bool result, string inAppId)
    {
        var purchaseItem = _purchasesLibrary.Items.Find(i => string.Equals(i.InAppId, inAppId, StringComparison.Ordinal));
        if (purchaseItem != null)
        {
            _coinsAmount += purchaseItem.CurrencyAmount;
            OnCoinsChanged?.Invoke();
        }
    }

    public int GetAvailableCoins()
    {
        return _coinsAmount;
    }


    public void ConsumeCoins(int count)
    {
        _coinsAmount -= count;
        OnCoinsChanged?.Invoke();
    }
}