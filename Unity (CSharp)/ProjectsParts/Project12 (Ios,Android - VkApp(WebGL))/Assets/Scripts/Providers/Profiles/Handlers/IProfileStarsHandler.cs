using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles.Handlers
{
	public interface IProfileStarsHandler
	{
		/// <summary>
		/// Событие изменения числа звезд
		/// int - Новое число звезд
		/// int - Общее число звезд
		/// </summary>
		UnityEvent<int, int> OnStarsChange { get; set; }

		void AddStars(int stars);
	}
}