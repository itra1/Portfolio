using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.User
{
	/// <summary>
	/// Провайдер игрока
	/// </summary>
	public interface IUserProvider
	{
		/// <summary>
		/// Инициализировано
		/// </summary>
		bool IsInitiated { get; }
		/// <summary>
		/// Имя игрока
		/// </summary>
		string UserName { get; }
		string AvatarName { get; }
		/// <summary>
		/// Возраст игрока
		/// </summary>
		int AgeValue { get; }

		/// <summary>
		/// количество очков
		/// </summary>
		ulong PointsCount { get; }

		/// <summary>
		/// Имя уже установлено
		/// </summary>
		bool IsExistsName { get; }

		/// <summary>
		/// Инициализация
		/// </summary>
		public void Initiate();

		/// <summary>
		/// ВВод имени игрока
		/// </summary>
		/// <param name="name">Новое имя</param>
		bool EnterName(string name);

		void SetAvatarName(string nameAvatar);

		/// <summary>
		/// Добавление очков
		/// </summary>
		/// <param name="points"></param>
		void PointsAdd(ulong points);

		/// <summary>
		/// Вычитание очков
		/// </summary>
		/// <param name="points"></param>
		void PointsSubtract(ulong points);

	}
}
