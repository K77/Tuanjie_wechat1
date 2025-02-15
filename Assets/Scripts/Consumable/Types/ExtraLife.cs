﻿using UnityEngine;
using System;

public class ExtraLife : Consumable
{
    protected const int k_MaxLives = 3;
    protected const int k_CoinValue = 10;

    public override string GetConsumableName()
    {
        return "Life";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.EXTRALIFE;
    }

    public override int GetPrice()
    {
        return 2000;
    }

	public override int GetPremiumCost()
	{
		return 5;
	}

	public override void Started(CharacterInputController c)
    {
        base.Started(c);
        if (c.currentLife < k_MaxLives)
            c.currentLife += 1;
		else
            c.coins += k_CoinValue;
    }
}
