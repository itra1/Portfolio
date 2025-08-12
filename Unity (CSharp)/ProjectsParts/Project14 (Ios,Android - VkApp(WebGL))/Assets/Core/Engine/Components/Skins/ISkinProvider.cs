using Core.Engine.Components.Shop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.Skins
{
	public interface ISkinProvider
	{
		public List<ISkin> Items { get; }

		/// <summary>
		/// проверка на активный скин
		/// </summary>
		/// <param name="skinType">Тип</param>
		/// <param name="uuid">UUID</param>
		/// <returns></returns>
		bool IsActiveSkin(string skinType, string uuid);

		/// <summary>
		/// Получить активный скин
		/// </summary>
		/// <param name="skinType">Тип</param>
		/// <returns></returns>
		ISkin GetActiveSkin(string skinType);

		/// <summary>
		/// Установка активного скина
		/// </summary>
		/// <param name="skinType"></param>
		/// <param name="uuid"></param>
		void SetActiveSkin(string skinType, string uuid);
		/// <summary>
		/// Установка активного скина
		/// </summary>
		/// <param name="skin"></param>
		void SetActiveSkin(ISkin skin);

		/// <summary>
		/// Добавление доступного для выбора скина
		/// </summary>
		/// <param name="uuid"></param>
		void AddReadySkin(string uuid);

		/// <summary>
		/// Проверка что скин доступен для выбора
		/// </summary>
		/// <param name="uuid">UUID скина</param>
		/// <returns></returns>
		bool IsReadyToSelect(string uuid);
	}
}
