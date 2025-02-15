﻿using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopAccessoriesList : ShopList
{
    public GameObject headerPrefab;

    public override void Populate()
    {
		m_RefreshCallback = null;

		foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        foreach (KeyValuePair<string, Character> pair in CharacterDatabase.dictionary)
        {
            Character c = pair.Value;
            if (c != null && c.accessories !=null && c.accessories.Length > 0)
            {
                GameObject header = Instantiate(headerPrefab);
                header.transform.SetParent(listRoot, false);
                ShopItemListItem itmHeader = header.GetComponent<ShopItemListItem>();
                itmHeader.nameText.text = c.characterName;

                for (int i = 0; i < c.accessories.Length; ++i)
                {
                    CharacterAccessories accessory = c.accessories[i];
                    GameObject newEntry = Instantiate(prefabItem);
                    newEntry.transform.SetParent(listRoot, false);

                    ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();

                    string compoundName = c.characterName + ":" + accessory.accessoryName;

					itm.nameText.text = accessory.accessoryName;
					itm.pricetext.text = accessory.cost.ToString();
					itm.icon.sprite = accessory.accessoryIcon;
					itm.buyButton.image.sprite = itm.buyButtonSprite;

					if (accessory.premiumCost > 0)
					{
						itm.premiumText.transform.parent.gameObject.SetActive(true);
						itm.premiumText.text = accessory.premiumCost.ToString();
					}
					else
					{
						itm.premiumText.transform.parent.gameObject.SetActive(false);
					}

                    itm.buyButton.onClick.AddListener(delegate () { Buy(compoundName, accessory.cost, accessory.premiumCost); });

					m_RefreshCallback += delegate () { RefreshButton(itm, accessory, compoundName); };
					RefreshButton(itm, accessory, compoundName);
				}
            }
        }
    }

	protected void RefreshButton(ShopItemListItem itm, CharacterAccessories accessory, string compoundName)
	{
		if (accessory.cost > PlayerData.instance.coins)
		{
			itm.buyButton.interactable = false;
			itm.pricetext.color = Color.red;
		}
		else
		{
			itm.pricetext.color = Color.black;
		}

		if (accessory.premiumCost > PlayerData.instance.premium)
		{
			itm.buyButton.interactable = false;
			itm.premiumText.color = Color.red;
		}
		else
		{
			itm.premiumText.color = Color.black;
		}

		if (PlayerData.instance.characterAccessories.Contains(compoundName))
		{
			itm.buyButton.interactable = false;
			itm.buyButton.image.sprite = itm.disabledButtonSprite;
			itm.buyButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Owned";
		}
	}



	public void Buy(string name, int cost, int premiumCost)
    {
        PlayerData.instance.coins -= cost;
		PlayerData.instance.premium -= premiumCost;
		PlayerData.instance.AddAccessory(name);
        PlayerData.instance.Save();

#if UNITY_ANALYTICS
        if (cost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", name },
                { "amount", cost },
                { "type", "soft" },
                { "new_balance", PlayerData.instance.coins }
            });
        }

        if (premiumCost > 0)
        {
            Analytics.CustomEvent("currency_spent", new Dictionary<string, object>
            {
                { "item_name", name },
                { "amount", premiumCost },
                { "type", "hard" },
                { "new_balance", PlayerData.instance.premium }
            });
        }
#endif

        Refresh();
    }
}
