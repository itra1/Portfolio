using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

/// <summary>
/// Продоваемый продукт
/// </summary>
public abstract class ShopProductBehaviour : MonoBehaviour, IGuid {
	
	public string title;
	public string description;
	public int price;
	public Sprite icon;

	public abstract float count { get; }

	public virtual Sprite Icon {
		get { return icon; }
	}

	public string _guid;
	
	public string Guid {
		get { return _guid; }
		set { _guid = value; }
	}

	// Покупка
	public virtual bool Bye() {
		GameEvents.OnBye.Call(this);
		return true;
	}

	// Проверка перед покупкой
	protected virtual bool CheckBuy() {
		return price <= Game.User.UserManager.Instance.silverCoins.Value;
	}
	
}
