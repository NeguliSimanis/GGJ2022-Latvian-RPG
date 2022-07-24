using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAP : MonoBehaviour
{
    private string fullVersion = "com.simanismikoss.moonaria.fullversion";
    [SerializeField]
    PopupManager popupManager;

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
