using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopCharacterList : ShopList
{
    public override void Populate()
    {
		m_RefreshCallback = null;
        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        foreach(KeyValuePair<string, Character> pair in CharacterDatabase.dictionary)
        {
            Character c = pair.Value;
            if (c != null)
            {
                GameObject newEntry = Instantiate(prefabItem);
                newEntry.transform.SetParent(listRoot, false);

                ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();

				itm.icon.sprite = c.icon;
                itm.nameText.text = c.characterName;
				itm.pricetext.text = c.cost.ToString();

				itm.buyButton.image.sprite = itm.buyButtonSprite;

				if (c.premiumCost > 0)
				{
					itm.premiumText.transform.parent.gameObject.SetActive(true);
					itm.premiumText.text = c.premiumCost.ToString();
				}
				else
				{
					itm.premiumText.transform.parent.gameObject.SetActive(false);
				}

				itm.buyButton.onClick.AddListener(delegate () { Buy(c); });

				m_RefreshCallback += delegate() { RefreshButton(itm, c); };
				RefreshButton(itm, c);
            }
        }
    }

	protected void RefreshButton(ShopItemListItem itm, Character c)
	{
		if (c.cost > PlayerData.instance.coins)
		{
			itm.buyButton.interactable = false;
			itm.pricetext.color = Color.red;
		}
		else
		{
			itm.pricetext.color = Color.black;
		}

		if (c.premiumCost > PlayerData.instance.premium)
		{
			itm.buyButton.interactable = false;
			itm.premiumText.color = Color.red;
		}
		else
		{
			itm.premiumText.color = Color.black;
		}

		if (PlayerData.instance.characters.Contains(c.characterName))
		{
			itm.buyButton.interactable = false;
			itm.buyButton.image.sprite = itm.disabledButtonSprite;
			itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Owned";
		}
	}



	public void Buy(Character c)
    {
        PlayerData.instance.coins -= c.cost;
		PlayerData.instance.premium -= c.premiumCost;
        PlayerData.instance.AddCharacter(c.characterName);
        PlayerData.instance.Save();

#if UNITY_ANALYTICS
        if (c.cost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", c.characterName },
                { "amount", c.cost},
                { "type", "soft" },
                { "new_balance", PlayerData.instance.coins }
            });
        }

        if (c.premiumCost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", c.characterName },
                { "amount", c.premiumCost },
                { "type", "hard" },
                { "new_balance", PlayerData.instance.premium }
            });
        }
#endif

        // Repopulate to change button accordingly.
        Populate();
    }
}
