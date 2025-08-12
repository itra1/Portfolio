namespace Game.Providers.Timers.Common {
	public interface ITimerTick :ITimer {
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
		void EmitOnComplete();
		void EmitOnRemove();
	}
}
