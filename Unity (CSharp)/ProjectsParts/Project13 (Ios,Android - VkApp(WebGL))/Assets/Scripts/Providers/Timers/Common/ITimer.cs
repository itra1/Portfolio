using System;
using UnityEngine.Events;

namespace Game.Providers.Timers.Common
{
	public interface ITimer
	{
		/// <summary>
		/// Запущен
		/// </summary>
		bool IsActive { get; }
		/// <summary>
		/// На паузе
		/// </summary>
		bool IsPaused { get; }
		/// <summary>
		/// Автоматически удалять
		/// </summary>
		bool IsAutoRemove { get; }

		double CurrentValueSeconds { get; }

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

		ITimer Complete();

		ITimer End(double seconds);
		ITimer End(DateTime EndTime);

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
		ITimer OnComplete(UnityAction action);
		/// <summary>
		/// Событие удаления
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ITimer OnRemove(UnityAction action);
	}
}
