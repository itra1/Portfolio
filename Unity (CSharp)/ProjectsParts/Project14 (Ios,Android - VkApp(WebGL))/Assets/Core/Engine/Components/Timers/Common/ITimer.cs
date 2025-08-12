using UnityEngine.Events;

namespace Core.Engine.Components.Timers
{
	public interface ITimer
	{
	/// <summary>
	/// Запущен
	/// </summary>
		bool IsStarted { get; }
		/// <summary>
		/// На паузе
		/// </summary>
		bool IsPaused { get; }
		/// <summary>
		/// Остановлен
		/// </summary>
		bool IsStoped { get; }
		/// <summary>
		/// Автоматически удалять
		/// </summary>
		bool IsAutoRemove { get; }

		/// <summary>
		/// Запуск таймера
		/// </summary>
		ITimer Start(double startValue = 0);

		/// <summary>
		/// Рауза
		/// </summary>
		ITimer Pause(bool isPause);
		/// <summary>
		/// Остановка таймера
		/// </summary>
		ITimer Stop();

		/// <summary>
		/// Автоматически удалять после остановки
		/// </summary>
		/// <param name="isAutoRemove">Автоматически удалить</param>
		/// <returns></returns>
		ITimer AutoRemove(bool isAutoRemove = true);
		/// <summary>
		/// Событие запуска
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnStart(UnityAction action);
		/// <summary>
		/// События тика
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnTick(UnityAction<double> action);
		/// <summary>
		/// Событие паузы
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnPause(UnityAction<bool> action);
		/// <summary>
		/// Событие остановки
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnStop(UnityAction action);
		/// <summary>
		/// Событие удаления
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnRemove(UnityAction action);
	}
}
