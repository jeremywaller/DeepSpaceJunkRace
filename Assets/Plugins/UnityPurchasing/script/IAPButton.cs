#if UNITY_PURCHASING
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
	[RequireComponent (typeof (Button))]
	[AddComponentMenu("Unity IAP/IAP Button")]
	[HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
	public class IAPButton : MonoBehaviour
	{
		[System.Serializable]
		public class OnPurchaseCompletedEvent : UnityEvent<Product> {};

		[System.Serializable]
		public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason> {};

		[HideInInspector]
		public string productId;

		[Tooltip("Consume the product immediately after a successful purchase")]
		public bool consumePurchase = true;

		[Tooltip("Event fired after a successful purchase of this product")]
		public OnPurchaseCompletedEvent onPurchaseComplete;

		[Tooltip("Event fired after a failed purchase of this product")]
		public OnPurchaseFailedEvent onPurchaseFailed;

		[Tooltip("[Optional] Displays the localized title from the app store")]
		public Text titleText;

		[Tooltip("[Optional] Displays the localized description from the app store")]
		public Text descriptionText;

		[Tooltip("[Optional] Displays the localized price from the app store")]
		public Text priceText;

		void Start ()
		{
			Button button = GetComponent<Button>();
			if (button) {
				button.onClick.AddListener(PurchaseProduct);
			}

			if (string.IsNullOrEmpty(productId)) {
				Debug.LogError("IAPButton productId is empty");
			}

			if (!IAPButtonStoreManager.Instance.HasProductInCatalog(productId)) {
				Debug.LogWarning("The product catalog has no product with the ID \"" + productId + "\"");
			}
		}

		void OnEnable()
		{
			IAPButtonStoreManager.Instance.AddButton(this);

            Debug.Log(string.Format("Enabling button for product {0}", productId));

			var product = IAPButtonStoreManager.Instance.GetProduct(productId);
			if (product != null) {
				if (titleText != null) {
					titleText.text = product.metadata.localizedTitle;
				}

				if (descriptionText != null) {
					descriptionText.text = product.metadata.localizedDescription;
				}

				if (priceText != null) {
					priceText.text = product.metadata.localizedPriceString;
				}
			}

            Debug.Log(string.Format("Button ready for product {0}", productId));
        }

		void OnDisable()
		{
			IAPButtonStoreManager.Instance.RemoveButton(this);
		}

		void PurchaseProduct()
		{
			Debug.Log("IAPButton.PurchaseProduct() with product ID: " + productId);

			IAPButtonStoreManager.Instance.InitiatePurchase(productId);
		}

		/**
		 *  Invoked to process a purchase of the product associated with this button
		 */
		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
		{
            Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
            Debug.Log("Receipt: " + e.purchasedProduct.receipt);

			onPurchaseComplete.Invoke(e.purchasedProduct);

			return (consumePurchase) ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
		}

		/**
		 *  Invoked on a failed purchase of the product associated with this button
		 */
		public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
		{
            Debug.Log("Purchase failed: " + product.definition.id);
            Debug.Log(reason);

			onPurchaseFailed.Invoke(product, reason);
		}

		public class IAPButtonStoreManager : IStoreListener
		{
			private static IAPButtonStoreManager instance = new IAPButtonStoreManager();
			private ProductCatalog catalog;
			private List<IAPButton> activeButtons = new List<IAPButton>();
            private IAppleExtensions _appleExtensions;

            protected IStoreController controller;
			protected IExtensionProvider extensions;

			private IAPButtonStoreManager()
			{
				catalog = ProductCatalog.LoadDefaultCatalog();
                
				StandardPurchasingModule module = StandardPurchasingModule.Instance();
                module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

				ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

				foreach (var product in catalog.allProducts) {
					if (product.allStoreIDs.Count > 0) {
						var ids = new IDs();
						foreach (var storeID in product.allStoreIDs) {
							ids.Add(storeID.id, storeID.store);
						}
						builder.AddProduct(product.id, product.type, ids);
                        Debug.Log(string.Format("Add Product: {0}", product.id));
					} else {
						builder.AddProduct(product.id, product.type);
                        Debug.Log(string.Format("Add Product: {0}", product.id));
                    }
				}

				UnityPurchasing.Initialize (this, builder);
			}

			public static IAPButtonStoreManager Instance {
				get {
					return instance;
				}
			}

			public IStoreController StoreController {
				get {
					return controller;
				}
			}

			public IExtensionProvider ExtensionProvider {
				get {
					return extensions;
				}
			}

            /// <summary>
            /// iOS Specific.
            /// This is called as part of Apple's 'Ask to buy' functionality,
            /// when a purchase is requested by a minor and referred to a parent
            /// for approval.
            /// 
            /// When the purchase is approved or rejected, the normal purchase events
            /// will fire.
            /// </summary>
            /// <param name="item">Item.</param>
            private void OnDeferred(Product item)
            {
                Debug.Log("Purchase deferred: " + item.definition.id);
            }

            public bool HasProductInCatalog(string productID)
			{
				foreach (var product in catalog.allProducts) {
					if (product.id == productID) {
						return true;
					}
				}
				return false;
			}

			public Product GetProduct(string productID)
			{
				if (controller != null) {
					return controller.products.WithID(productID);
				}
				return null;
			}

			public void AddButton(IAPButton button)
			{
				activeButtons.Add(button);
			}

			public void RemoveButton(IAPButton button)
			{
				activeButtons.Remove(button);
			}

            public List<IAPButton> GetActiveButtons()
            {
                return activeButtons;
            }

			public void InitiatePurchase(string productID)
			{
				controller.InitiatePurchase(productID);
			}

			public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
			{
                Debug.Log("Purchasing initialized!");

                _appleExtensions = extensions.GetExtension<IAppleExtensions>();
                // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
                // On non-Apple platforms this will have no effect; OnDeferred will never be called.
                _appleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

                this.controller = controller;
				this.extensions = extensions;

                RefreshButtonsText();
			}

			public void OnInitializeFailed (InitializationFailureReason error)
			{
                Debug.Log("Purchasing failed to initialize!");

                switch (error)
                {
                    case InitializationFailureReason.AppNotKnown:
                        Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                        break;
                    case InitializationFailureReason.PurchasingUnavailable:
                        // Ask the user if billing is disabled in device settings.
                        Debug.Log("Billing disabled!");
                        break;
                    case InitializationFailureReason.NoProductsAvailable:
                        // Developer configuration error; check product metadata.
                        Debug.Log("No products available for purchase!");
                        break;
                }
            }

			public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
			{
				foreach (var button in activeButtons) {
					if (button.productId == e.purchasedProduct.definition.id) {
						return button.ProcessPurchase(e);
					}
				}

				return PurchaseProcessingResult.Complete; // TODO: Maybe this shouldn't return complete
			}

			public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
			{ 
				foreach (var button in activeButtons) {
					if (button.productId == product.definition.id) {
						button.OnPurchaseFailed(product, reason);
					}
				} 
			}

            private void RefreshButtonsText()
            {
                foreach (IAPButton button in GetActiveButtons())
                {
                    Debug.Log(string.Format("Refreshing button for product {0}", button.productId));

                    var product = GetProduct(button.productId);
                    Debug.Log(string.Format("Store title: {0}", product.metadata.localizedTitle));
                    Debug.Log(string.Format("Store description: {0}", product.metadata.localizedDescription));
                    Debug.Log(string.Format("Store price: {0}", product.metadata.localizedPriceString));

                    if (product != null)
                    {
                        if (button.titleText != null)
                        {
                            button.titleText.text = product.metadata.localizedTitle;
                            Debug.Log(string.Format("New button title: {0}", button.titleText.text));
                        }

                        if (button.descriptionText != null)
                        {
                            button.descriptionText.text = product.metadata.localizedDescription;
                            Debug.Log(string.Format("New button description: {0}", button.descriptionText.text));
                        }

                        if (button.priceText != null)
                        {
                            button.priceText.text = product.metadata.localizedPriceString;
                            Debug.Log(string.Format("New button price: {0}", button.priceText.text));
                        }
                    }

                    Debug.Log(string.Format("Button refreshed for product {0}", button.productId));
                }
            }
		}
	}
}
#endif
