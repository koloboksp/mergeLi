using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

public class PurchaseController : IDetailedStoreListener
{
    private IStoreController _store;
    private IExtensionProvider _extensions;
    private IAppleExtensions _appleExtensions;
    private IGooglePlayStoreExtensions _googleExtensions;

    private bool _purchaseInProgress;
    private bool _purchaseResult;
        
    private CrossPlatformValidator _validator;

    private bool _initialized;

    public async Task InitializeAsync(IEnumerable<string> availableProducts)
    {
#if UNITY_EDITOR
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#elif UNITY_ANDROID || UNITY_IOS
            mValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#elif UNITY_STANDALONE
            return;            
#endif
        var configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        string storeName = null;
#if UNITY_ANDROID
        storeName = GooglePlay.Name;
#elif UNITY_IOS
            storeName = AppleAppStore.Name;
#elif UNITY_EDITOR
            storeName = "Standalone";
#endif
        foreach (var productId in availableProducts)
            configurationBuilder.AddProduct(productId, ProductType.Consumable, new IDs { { productId, storeName } });
           
        UnityPurchasing.Initialize(this, configurationBuilder);
    }
        
    public void OnInitialized(IStoreController store, IExtensionProvider extensions)
    {
        _store = store;
        _extensions = extensions;
        _appleExtensions = extensions.GetExtension<IAppleExtensions>();
        _googleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        _initialized = true;
        
        Debug.Log("<color=#00CCFF>IAP initialization success.</color>");
            
        /*
            foreach (var item in controller.products.all)
            {     
                Debug.Log($"id: {item.definition.id}.");
                Debug.Log($"availableToPurchase: {item.availableToPurchase}.");
                Debug.Log($"item.hasReceipt: {item.hasReceipt}.");
                Debug.Log($"item.transactionId: {item.transactionID}.");
            }
            */
    }
       
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        _initialized = true;
        Debug.LogException(new Exception( $"IAP initialize failed: {error}. Message: {message}"));
    }
        
    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }
       
    private bool Validate(string receipt, out string orderId)
    {
#if UNITY_EDITOR
        orderId = "XXXX-XXXX-XXXX-XXXX";
        return true;
#else
            orderId = string.Empty;
            
            if(receipt == null)
                return false;

            try
            {
                var data = _validator.Validate(receipt);
                /*
                Debug.Log($"receipt: {receipt}.");
                */
                foreach (IPurchaseReceipt productReceipt in data)
                {
                    /*
                    Debug.Log($"productId: {productReceipt.productID}.");
                    Debug.Log($"purchaseDate: {productReceipt.purchaseDate}.");
                    Debug.Log($"transactionId: {productReceipt.transactionID}.");
                    */

                    if (productReceipt is GooglePlayReceipt google)
                    {
                        /*
                        Debug.Log($"google.purchaseState: {google.purchaseState}.");
                        Debug.Log($"google.purchaseToken: {google.purchaseToken}.");
                        */
                    }

                    if (productReceipt is AppleInAppPurchaseReceipt apple)
                    {
                        /*
                        Debug.Log($"apple.originalTransactionIdentifier: {apple.originalTransactionIdentifier}.");
                        Debug.Log($"apple.subscriptionExpirationDate: {apple.subscriptionExpirationDate}.");
                        Debug.Log($"apple.cancellationDate: {apple.cancellationDate}.");
                        Debug.Log($"apple.quantity: {apple.quantity}.");
                        */
                    }

                    if (AvailableProductIds.Contains(productReceipt.productID))
                    {
                        if (productReceipt.transactionID != null)
                        {
                            orderId = productReceipt.transactionID;
                            return true;
                        }
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                Debug.LogException(new Exception("CrossPlatformValidator not implemented.", ex));
            }
            catch
            {
                Debug.LogException(new Exception("Invalid receipt, not unlocking content."));
            }
            
            return false;
#endif
    }
        
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"ProcessPurchase: {args.purchasedProduct.definition.id}.");

        _purchaseResult = Validate(args.purchasedProduct.receipt, out var orderId);
        _purchaseInProgress = false;

        return PurchaseProcessingResult.Complete;
    }
        
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"OnPurchaseFailed: product {failureReason}.");
    }
        
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"OnPurchaseFailed: product {failureDescription.productId}, reason {failureDescription.reason}, message {failureDescription.message}.");
            
        _purchaseInProgress = false;
        _purchaseResult = false;
    }
       
    public async Task<bool> Buy(string productId)
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS

        while (!_initialized)
            await Task.Yield();

        if (_store == null || _purchaseInProgress)
            return false;

        _purchaseInProgress = true;
        _purchaseResult = false;

        var product = _store.products.WithID(productId);
        if(product != null)
            _store.InitiatePurchase(productId);
        else
        {
            Debug.LogError($"Product with id '{productId} not found.'. Purchase operation break.");
            _purchaseInProgress = false;
            _purchaseResult = false;
        }
        
        while (_purchaseInProgress)
            await Task.Yield();

        return _purchaseResult;
#else
            return await Task.FromException<bool>(new Exception("This platform is not supported."));
#endif
    }
}