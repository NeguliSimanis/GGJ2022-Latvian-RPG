//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Purchasing;
////using UnityEngine.Purchasing.Extension;
//using UnityEngine.UI;


///// <summary>

///// yt video: https://www.youtube.com/watch?v=QKyLkLjr46k&ab_channel=GleyGames
///// unity example project: https://forum.unity.com/threads/sample-iap-project.529555/
///// 
///// other info sources that might not be used
///// /// tutorial: https://zditect.com/code/android/use-the-google-play-billing-library-with-unity-nbspnbsp-google-plays-billing-system-nbspnbsp-android-developers.html
///// forum post: https://github.com/google/play-unity-plugins/issues/52
///// </summary>




//public class IAP : MonoBehaviour, IStoreListener
//{
//    private string fullVersion = "com.simanismikoss.moonaria.fullversion";
    
//    [SerializeField]
//    PopupManager popupManager;

//    ConfigurationBuilder builder;

//#region copy pasta
//    private static IStoreController m_StoreController;          // The Unity Purchasing system.
//    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
//    private static Product test_product = null;


//    public static string GOLD_50 = "gold50";
//    public static string NO_ADS = "noads";
//    public static string SUB1 = "subscription1";

//    private static Text myText;

//    private bool return_complete = true;
//    #endregion

//    #region MYCODE
//    //private void Awake()
//    //{
//    //    if (Application.platform == RuntimePlatform.Android)
//    //    {
//    //        builder = ConfigurationBuilder.Instance(Google.Play.Billing.GooglePlayStoreModule.Instance());
//    //    }
//    //}

//    //public void OnPurchaseComplete(Product product)
//    //{
//    //    if (product.definition.id == fullVersion)
//    //    {
//    //        GameData.current.hasFullVersion = true;
//    //        popupManager.ShowUnlockFullVerPopup(false);
//    //    }
//    //}


//    #endregion NOCODE


//    void Start()
//    {
//        myText = GameObject.Find("MyText").GetComponent<Text>();

//        // If we haven't set up the Unity Purchasing reference
//        if (m_StoreController == null)
//        {
//            // Begin to configure our connection to Purchasing
//            InitializePurchasing();
//        }
//        MyDebug("Complete = " + return_complete.ToString());
//    }

//    public void InitializePurchasing()
//    {
//        if (IsInitialized())
//        {
//            return;
//        }

//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//        builder.AddProduct(GOLD_50, ProductType.Consumable);
//        builder.AddProduct(NO_ADS, ProductType.NonConsumable);
//        builder.AddProduct(SUB1, ProductType.Subscription);

//        UnityPurchasing.Initialize(this, builder);
//    }


//    private bool IsInitialized()
//    {
//        return m_StoreController != null && m_StoreExtensionProvider != null;
//    }

//    public void BuySubscription()
//    {
//        BuyProductID(SUB1);
//    }

//    public void BuyGold50()
//    {
//        BuyProductID(GOLD_50);
//    }

//    public void BuyNoAds()
//    {
//        BuyProductID(NO_ADS);
//    }

//    public void CompletePurchase()
//    {
//        if (test_product == null)
//            MyDebug("Cannot complete purchase, product not initialized.");
//        else
//        {
//            m_StoreController.ConfirmPendingPurchase(test_product);
//            MyDebug("Completed purchase with " + test_product.transactionID.ToString());
//        }

//    }

//    public void ToggleComplete()
//    {
//        return_complete = !return_complete;
//        MyDebug("Complete = " + return_complete.ToString());

//    }
//    public void RestorePurchases()
//    {
//        m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
//            if (result)
//            {
//                MyDebug("Restore purchases succeeded.");
//            }
//            else
//            {
//                MyDebug("Restore purchases failed.");
//            }
//        });
//    }

//    void BuyProductID(string productId)
//    {
//        if (IsInitialized())
//        {
//            Product product = m_StoreController.products.WithID(productId);

//            if (product != null && product.availableToPurchase)
//            {
//                MyDebug(string.Format("Purchasing product:" + product.definition.id.ToString()));
//                m_StoreController.InitiatePurchase(product);
//            }
//            else
//            {
//                MyDebug("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
//            }
//        }
//        else
//        {
//            MyDebug("BuyProductID FAIL. Not initialized.");
//        }
//    }

//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        MyDebug("OnInitialized: PASS");

//        m_StoreController = controller;
//        m_StoreExtensionProvider = extensions;
//    }


//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
//        MyDebug("OnInitializeFailed InitializationFailureReason:" + error);
//    }


//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        test_product = args.purchasedProduct;



//        //MyDebug(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));

//        if (return_complete)
//        {
//            MyDebug(string.Format("ProcessPurchase: Complete. Product:" + args.purchasedProduct.definition.id + " - " + test_product.transactionID.ToString()));
//            return PurchaseProcessingResult.Complete;
//        }
//        else
//        {
//            MyDebug(string.Format("ProcessPurchase: Pending. Product:" + args.purchasedProduct.definition.id + " - " + test_product.transactionID.ToString()));
//            return PurchaseProcessingResult.Pending;
//        }

//    }


//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        MyDebug(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
//    }

//    private void MyDebug(string debug)
//    {

//        Debug.Log(debug);
//        myText.text += "\r\n" + debug;
//    }

//}
