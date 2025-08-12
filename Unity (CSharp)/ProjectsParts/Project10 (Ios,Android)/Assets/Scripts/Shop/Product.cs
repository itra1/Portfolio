using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

namespace Shop.Products {
	
	/// <summary>
	/// Базовый класс продукта магазина
	/// </summary>
	public abstract class Product: MonoBehaviour {

		public string title;
		public ProductType type;
		public string id;
		public string displayTitle;
		public string displayDescription;
		public Sprite sprite;
		
		public abstract Price GetPrice();

		public abstract void Bye(System.Action<Product, bool> callback = null);

		public virtual int GetLevel() {
			return PlayerPrefs.GetInt(id, 0);
		}

		public virtual void SetLevel(int value) {
			PlayerPrefs.SetInt(id, value);
		}

		public abstract bool ByePossible();

		/// <summary>
		/// Это полностью куплено
		/// </summary>
		/// <returns></returns>
		public abstract bool IsBye();

		protected void Log() {
			YAppMetrica.Instance.ReportEvent("Магазин: куплено " + LanguageManager.GetTranslate(id));
			GAnalytics.Instance.LogEvent("Магазин", "Покупка", LanguageManager.GetTranslate(id), 1);
		}

	}

	[System.Serializable]
	public struct Price {
		public int coins;
	}
	[Flags]
	public enum ProductType {
		none = 0,
		upgrade = 2,
		mount = 4,
		gadget = 8,
		clothHead = 16,
		clothSpine = 32,
		clothAccessory = 64,
		cloth = clothHead | clothSpine | clothAccessory,
		all = upgrade | mount | gadget | cloth
	}

}