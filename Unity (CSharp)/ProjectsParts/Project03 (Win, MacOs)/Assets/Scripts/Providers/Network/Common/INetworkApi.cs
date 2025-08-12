using Cysharp.Threading.Tasks;

using Providers.Network.Materials;

namespace Providers.Network.Common
{
	public interface INetworkApi
	{
		/// <summary>
		/// Авторизация
		/// </summary>
		/// <param name="username">Пользователь email или phone (+7*****)</param>
		/// <param name="password">Пароль</param>
		/// <returns></returns>
		UniTask<(bool, object)> Authorization(string username, string password);
		/// <summary>
		/// Регистрация пользователя
		/// </summary>
		/// <param name="regData"></param>
		/// <returns></returns>
		UniTask<(bool, object)> Registration(object regData);
		/// <summary>
		/// Получение списка стран
		/// </summary>
		/// <returns></returns>
		UniTask<(bool, object)> GetCountries();
		/// <summary>
		/// Получение списка валют
		/// </summary>
		/// <returns></returns>
		UniTask<(bool, object)> GetCurrency();

		/// <summary>
		/// Запрос pin
		/// </summary>
		/// <param name="data">{email | phone}</param>
		/// <returns></returns>
		UniTask<(bool, object)> GetPinRestorePassword(object data);
		/// <summary>
		/// Проверка Pin
		/// </summary>
		/// <param name="data">{phone, code}</param>
		/// <returns></returns>
		UniTask<(bool, object)> ChechPinPassword(object data);
		/// <summary>
		/// Запрос обновления пароля
		/// </summary>
		/// <param name="data">данные</param>
		/// <returns></returns>
		UniTask<(bool, object)> RequestUpdatePassword();
		/// <summary>
		/// Обновление пароль
		/// </summary>
		/// <param name="data">{password, user, code}</param>
		/// <returns></returns>
		UniTask<(bool, object)> PasswordRestore(object data);
	}
}
