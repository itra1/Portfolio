using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Garilla
{
	/// <summary>
	/// Менеджер таймеров
	/// </summary>
	public class TimerManager : Singleton<TimerManager>
	{
		private List<Timer> _timers = new List<Timer>();
		private object _locker = new object();

		public Timer AddTimer(string name, float time, bool isReal = true)
		{
			Timer t = AddTimer(time, isReal);
			t.Name = name;
			return t;
		}
		public Timer AddTimer(float time, bool isReal = true)
		{
			Timer t = isReal ? new RealTimer() : new GameTimer();
			t.Set(time);
			lock (_locker)
				_timers.Add(t);
			return t;
		}

		public Timer GetTimer(string name)
		{
			return _timers.Find(x => x.Name == name);
		}

		public void RemoveTimer(Timer timer)
		{
			lock (_locker)
				_timers.Remove(timer);
			it.Logger.Log("Remove timer " + timer.Name);
			timer = null;
		}

		private void Update()
		{
			lock (_locker)
				_timers.ForEach(t =>
			{
				t.Update();
			});
		}

		public class Timer
		{
			//public UnityEngine.Events.UnityAction OnStart;
			public UnityEngine.Events.UnityEvent OnUpdate = new UnityEngine.Events.UnityEvent();
			public UnityEngine.Events.UnityEvent<bool> OnComplete = new UnityEngine.Events.UnityEvent<bool>();

			public string Name;

			public virtual bool IsActive { get; private set; }

			public virtual void Set(float time) { }

			public virtual void Play()
			{
				//OnStart?.Invoke();
			}

			public virtual void Update()
			{
				OnUpdate?.Invoke();
			}

			public virtual void Complete()
			{
				OnComplete?.Invoke(true);
				TimerManager.Instance.RemoveTimer(this);
			}

			public virtual float TimeLeft => 0;

			public virtual void Stop()
			{
				OnComplete?.Invoke(false);
				TimerManager.Instance.RemoveTimer(this);
			}

		}

		public class GameTimer : Timer
		{
			private float _targetTime;

			public override float TimeLeft => _targetTime - Time.timeSinceLevelLoad;

			public override void Set(float time)
			{
				_targetTime = Time.timeSinceLevelLoad + time;
			}

			public override void Update()
			{
				base.Update();

				if (_targetTime < Time.timeSinceLevelLoad)
				{
					Complete();
				}

			}
		}

		public class RealTimer : Timer
		{
			private DateTime _targetTime;
			public double TimeLeftDouble => Math.Max(0, (_targetTime - GameHelper.NowTime).TotalSeconds);

			public override bool IsActive => TimeLeftDouble > 0;

			public override void Set(float time)
			{
				_targetTime = GameHelper.NowTime.AddSeconds(time);
			}

			public override void Update()
			{
				base.Update();

				if (GameHelper.NowTime > _targetTime)
				{
					Complete();
				}

			}

		}

	}
}