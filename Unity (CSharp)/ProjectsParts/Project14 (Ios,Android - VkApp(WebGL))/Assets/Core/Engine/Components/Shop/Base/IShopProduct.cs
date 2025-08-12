using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Core.Engine.Components.Shop
{
	public interface IShopProduct
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		string UUID { get; }

		/// <summary>
		/// Платформ
		/// </summary>
		PlatformSystemPlatformType Platform { get; }

		/// <summary>
		/// Название
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Стоимость в очках
		/// </summary>
		public ulong Price { get; }

		/// <summary>
		/// Лписание товара
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Группа покупок
		/// </summary>
		string GroupType { get; }

		/// <summary>
		/// Иконка
		/// </summary>
		Sprite Icone { get; }

		/// <summary>
		/// Показывать в магазине для покупок
		/// </summary>
		bool IsVisibleShop { get; }

		/// <summary>
		/// Готов к покупку
		/// </summary>
		bool IsBuyReady { get; }

		/// <summary>
		/// Уже куплено
		/// </summary>
		bool IsAlreadyBuyed { get; }

		/// <summary>
		/// Купить
		/// </summary>
		/// <returns>Успешная покупка</returns>
		bool Buy();

		void SubscribeChange(UnityAction action);
		void UnSubscribeChange(UnityAction action);

	}
}
