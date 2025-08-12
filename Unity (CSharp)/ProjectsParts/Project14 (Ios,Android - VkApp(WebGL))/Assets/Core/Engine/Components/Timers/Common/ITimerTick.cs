using System;
using System.Collections.Generic;
namespace Core.Engine.Components.Timers
{
	internal interface ITimerTick : ITimer
	{
		/// <summary>
		/// Событие тика
		/// </summary>
		ITimer Tick();
		/// <summary>
		/// Удалить
		/// </summary>
		/// <returns></returns>
		ITimer Remove();

		void EmitOnStart();
		void EmitOnTick(double currentValue);
		void EmitOnPause(bool isPaused);
		void EmitOnStop();
		void EmitOnRemove();
	}
}
