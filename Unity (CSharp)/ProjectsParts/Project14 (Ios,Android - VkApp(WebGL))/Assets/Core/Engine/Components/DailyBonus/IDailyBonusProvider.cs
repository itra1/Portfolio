using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.DailyBonus
{
	public interface IDailyBonusProvider
	{
		/// <summary>
		/// Количество уже полученных дневных бонусов
		/// </summary>
		int DaysGet { get; }

		/// <summary>
		/// Готовность получить новый дневной бонус
		/// </summary>
		bool ReadyNewGet { get; }

		/// <summary>
		/// Добавить день в бонусы
		/// </summary>
		public void AddDay();

	}
}
