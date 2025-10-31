using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using Product = UnityEngine.Purchasing.Product;

namespace Core.Market
{
    public class PurchaseController
    {
        private StoreController _storeController;
        
        private bool _purchaseInProgress;
        private bool _purchaseResult;
        
        private CrossPlatformValidator _validator;

        private bool _initialized;
        private readonly List<string> _availableProducts = new List<string>();
        
        public async Task InitializeAsync(IEnumerable<string> availableProducts)
        {
            try
            {
                Debug.Log($"<color=#99ff99>Initialize {nameof(PurchaseController)}.</color>");
                var timer = new Atom.Timers.SmallTimer();

                _availableProducts.AddRange(availableProducts);
#if UNITY_EDITOR
               
#elif UNITY_ANDROID || UNITY_IOS
                _validator = new CrossPlatformValidator(UnityEngine.Purchasing.Security.GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#elif UNITY_STANDALONE
                return; 
#else

#endif
          
#if UNITY_WEBGL && !UNITY_EDITOR

#else
                _storeController = UnityIAPServices.StoreController();
                _storeController.OnPurchasePending += OnPurchasePending;
                _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                _storeController.OnPurchaseFailed += OnPurchaseFailed;
               
                _storeController.OnStoreDisconnected += OnStoreDisconnected;
                
                _storeController.OnProductsFetched += OnProductsFetched;
                _storeController.OnProductsFetchFailed += OnProductsFetchedFailed;
                
                await _storeController.Connect();

                var initialProductsToFetch = new List<ProductDefinition>();
                foreach (var productId in _availableProducts)
                {
                    initialProductsToFetch.Add(new(productId, ProductType.Consumable));
                }
                
                _storeController.FetchProducts(initialProductsToFetch);
#endif
                Debug.Log($"<color=#99ff99>Time initialize {nameof(PurchaseController)}: {timer.Update()}.</color>");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private void OnProductsFetched(List<Product> products)
        {
            Debug.Log("<color=#00CCFF>Products fetched.</color>");
            _initialized = true;
        }

        private void OnProductsFetchedFailed(ProductFetchFailed failure)
        {
            Debug.LogException(new Exception( $"Products fetch failed for {failure.FailedFetchProducts.Count} products: {failure.FailureReason}"));
        }
        
        private void OnPurchasePending(PendingOrder order)
        {
            var product = GetFirstProductInOrder(order);
            if (product is null)
            {
                Debug.LogError("Could not find product in order.");
                return;
            }

            _purchaseResult = Validate(product.receipt, out var orderId);
            _purchaseInProgress = false;
            
            Debug.Log($"Purchase complete. Product: {product.definition.id}, Result: {_purchaseResult}");

            _storeController.ConfirmPurchase(order);
        }
        
        private void OnPurchaseConfirmed(Order order)
        {
            switch (order)
            {
                case ConfirmedOrder confirmedOrder:
                    OnPurchaseConfirmed(confirmedOrder);
                    break;
                case FailedOrder failedOrder:
                    OnPurchaseConfirmationFailed(failedOrder);
                    break;
                default:
                    Debug.Log("Unknown OnPurchaseConfirmed result.");
                    break;
            }
        }
        
        private void OnPurchaseConfirmed(ConfirmedOrder order)
        {
            var product = GetFirstProductInOrder(order);
            if (product == null)
            {
                Debug.LogError("Could not find product in purchase confirmation.");
                return;
            }

            Debug.Log($"Purchase confirmed. Product: {product?.definition.id}");
        }
        
        private void OnPurchaseConfirmationFailed(FailedOrder order)
        {
            var product = GetFirstProductInOrder(order);
            if (product == null)
            {
                Debug.LogError("Could not find product in failed confirmation.");
            }

            Debug.LogError($"Confirmation failed - Product: '{product?.definition.id}'," +
                           $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                           + $"Confirmation Failure Details: {order.Details}");
        }
        
        private void OnPurchaseFailed(FailedOrder order)
        {
            _purchaseResult = false;
            _purchaseInProgress = false;
            
            var product = GetFirstProductInOrder(order);
            if (product == null)
            {
                Debug.LogError("Could not find product in failed order.");
            }

            Debug.LogError($"Purchase failed - Product: '{product?.definition.id}'," +
                           $"PurchaseFailureReason: {order.FailureReason.ToString()},"
                           + $"Purchase Failure Details: {order.Details}");
        }
        
        private  Product GetFirstProductInOrder(Order order)
        {
            return order.CartOrdered.Items().First()?.Product;
        }
        
        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            Debug.LogWarning($"Store disconnected details: {description.message}");
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

                    if (_availableProducts.Contains(productReceipt.productID))
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

        public async Task<bool> Buy(string productId, CancellationToken cancellationToken)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS

            while (!_initialized)
                await Task.Yield();

            if (_purchaseInProgress)
                return false;

            _purchaseInProgress = true;
            _purchaseResult = false;

            var product = _storeController.GetProductById(productId);
            if (product != null)
            {
                _storeController.PurchaseProduct(productId);
            }
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

        public string GetLocalizedPriceString(string productId)
        {
            if (!_initialized)
            {
                return String.Empty;
            }
            
            var product = _storeController.GetProductById(productId);
            if (product != null)
            {
                if (product.metadata != null)
                    return product.metadata.localizedPriceString;

                return "--";
            }

            return "--";
        }

        public bool IsProductAvailable(string productId)
        {
            if (!_initialized)
            {
                return false;
            }

            var product = _storeController.GetProductById(productId);
            if (product == null)
            {
                return false;
            }

            return product.availableToPurchase;
        }
    }
}