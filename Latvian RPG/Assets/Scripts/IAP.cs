using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

/// <summary>
/// tutorial: https://zditect.com/code/android/use-the-google-play-billing-library-with-unity-nbspnbsp-google-plays-billing-system-nbspnbsp-android-developers.html
/// forum post: https://github.com/google/play-unity-plugins/issues/52
/// yt video: https://www.youtube.com/watch?v=QKyLkLjr46k&ab_channel=GleyGames
/// </summary>

public class IAP : MonoBehaviour
{
    private string fullVersion = "com.simanismikoss.moonaria.fullversion";
    [SerializeField]
    PopupManager popupManager;

    ConfigurationBuilder builder;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            builder = ConfigurationBuilder.Instance(Google.Play.Billing.GooglePlayStoreModule.Instance());
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == fullVersion)
        {
            GameData.current.hasFullVersion = true;
            popupManager.ShowUnlockFullVerPopup(false);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("purchase failed");
    }
}
