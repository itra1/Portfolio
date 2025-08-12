using System;
using UnityEngine.Events;

namespace Game.Providers.Timers.Common {
	public abstract class Timer : ITimerTick {
		protected UnityEvent _OnStart = new();
		protected UnityEvent<double> _OnTick = new();
		protected UnityEvent<bool> _OnPause = new();
		protected UnityEvent _OnStop = new();
		protected UnityEvent _OnRemove = new();
		protected UnityEvent _OnComplete = new();

		protected bool _isActive = false;
		protected bool _isPaused = false;
		protected bool _autoRemove = true;

		public bool IsActive => _isActive;
		public bool IsPaused => _isPaused;
		public bool IsAutoRemove => _autoRemove;

		public abstract double CurrentValueSeconds { get; }

		public abstract ITimer Tick();

		public virtual ITimer Start(double startValue = 0) {
			_isActive = true;
			EmitOnStart();
			return this;
		}

		public virtual ITimer End(double EndTime) {
			return this;
		}
		public virtual ITimer End(DateTime EndTime) {
			return this;
		}

		public ITimer Pause(bool isPause) {
			_isPaused = isPause;
			EmitOnPause(_isPaused);
			return this;
		}

		public ITimer Complete() {
			EmitOnComplete();
			return this;
		}

		public ITimer Stop() {
			_isActive = false;
			EmitOnStop();
			return this;
		}

		public abstract ITimer Remove();

		public ITimer OnStart(UnityAction action) {
			_OnStart.AddListener(action);
			return this;
		}
		public ITimer OnComplete(UnityAction action) {
			_OnComplete.AddListener(action);
			return this;
		}

		public ITimer OnTick(UnityAction<double> action) {
			_OnTick.AddListener(action);
			return this;
		}

		public ITimer OnPause(UnityAction<bool> action) {
			_OnPause.AddListener(action);
			return this;
		}

		public ITimer OnStop(UnityAction action) {
			_OnStop.AddListener(action);
			return this;
		}

		public ITimer OnRemove(UnityAction action) {
			_OnRemove.AddListener(action);
			return this;
		}

		public ITimer AutoRemove(bool isAutoRemove = true) {
			_autoRemove = isAutoRemove;
			return this;
		}

		public void EmitOnStart() {
			_OnStart?.Invoke();
		}

		public void EmitOnTick(double currentValue) {
			_OnTick?.Invoke(currentValue);
		}

		public void EmitOnPause(bool isPaused) {
			_OnPause?.Invoke(isPaused);
		}

		public void EmitOnStop() {
			_OnStop?.Invoke();
		}

		public void EmitOnRemove() {
			_OnRemove?.Invoke();
		}

		public void EmitOnComplete() {
			_OnComplete?.Invoke();
		}
	}
}
