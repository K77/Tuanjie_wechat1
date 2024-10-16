using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopThemeList : ShopList
{
    public override void Populate()
    {
		m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        foreach (KeyValuePair<string, ThemeData> pair in ThemeDatabase.dictionnary)
        {
            ThemeData theme = pair.Value;
            if (theme != null)
            {
                GameObject newEntry = Instantiate(prefabItem);
                newEntry.transform.SetParent(listRoot, false);

                ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();

                itm.nameText.text = theme.themeName;
				itm.pricetext.text = theme.cost.ToString();
				itm.icon.sprite = theme.themeIcon;

				if (theme.premiumCost > 0)
				{
					itm.premiumText.transform.parent.gameObject.SetActive(true);
					itm.premiumText.text = theme.premiumCost.ToString();
				}
				else
				{
					itm.premiumText.transform.parent.gameObject.SetActive(false);
				}

				itm.buyButton.onClick.AddListener(delegate () { Buy(theme); });

				itm.buyButton.image.sprite = itm.buyButtonSprite;

				RefreshButton(itm, theme);
				m_RefreshCallback += delegate () { RefreshButton(itm, theme); };
            }
        }
    }

	protected void RefreshButton(ShopItemListItem itm, ThemeData theme)
	{
		if (theme.cost > PlayerData.instance.coins)
		{
			itm.buyButton.interactable = false;
			itm.pricetext.color = Color.red;
		}
		else
		{
			itm.pricetext.color = Color.black;
		}

		if (theme.premiumCost > PlayerData.instance.premium)
		{
			itm.buyButton.interactable = false;
			itm.premiumText.color = Color.red;
		}
		else
		{
			itm.premiumText.color = Color.black;
		}

		if (PlayerData.instance.themes.Contains(theme.themeName))
		{
			itm.buyButton.interactable = false;
			itm.buyButton.image.sprite = itm.disabledButtonSprite;
			itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Owned";
		}
	}


	public void Buy(ThemeData t)
    {
        PlayerData.instance.coins -= t.cost;
		PlayerData.instance.premium -= t.premiumCost;
        PlayerData.instance.AddTheme(t.themeName);
        PlayerData.instance.Save();

#if UNITY_ANALYTICS
        if (t.cost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", t.themeName },
                { "amount", t.cost },
                { "type", "soft" },
                { "new_balance", PlayerData.instance.coins }
            });
        }

        if (t.premiumCost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", t.themeName },
                { "amount", t.premiumCost },
                { "type", "hard" },
                { "new_balance", PlayerData.instance.premium }
            });
        }
#endif

        // Repopulate to change button accordingly.
        Populate();
    }
}
