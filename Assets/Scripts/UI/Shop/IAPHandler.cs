using System.Collections.Generic;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class IAPHandler : MonoBehaviour
{
#if UNITY_PURCHASING
    private void OnEnable()
    {
#if UNITY_ANALYTICS
        Analytics.CustomEvent("store_opened", new Dictionary<string, object>
        {
            { "type", "hard" }
        });
#endif
    }

    public void ProductBought(Product product)
    {
        int amount = 0;
        switch (product.definition.id)
        {
            case "10_premium":
                amount = 10;
                break;
            case "50_premium":
                amount = 50;
                break;
            case "100_premium":
                amount = 100;
                break;
        }

        if (amount > 0)
        {
            PlayerData.instance.premium += amount;
            PlayerData.instance.Save();

            Analytics.CustomEvent("currency_acquired", new Dictionary<string, object>
            {
                {"item_name", product.definition.id},
                {"amount",  amount},
                {"source", "store" },
                {"new_balance",   PlayerData.instance.premium},
                {"type", "hard"}
            });
        }
    }

    public void ProductError(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("Product : " + product.definition.id + " couldn't be bought because " + reason);
    }
#endif
}
