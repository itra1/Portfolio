using System;

using UnityEngine.Events;

using Zenject;

namespace Core.Engine.Components.GameQuests
{
	public abstract class GameQuest : IGameQuest, IDisposable
	{
		protected UnityEvent _OnComplete = new();
		protected UnityEvent _OnStart = new();
		protected SignalBus _signalBus;
		protected bool _isStarted;

		[Inject]
		public void InitBase(SignalBus signalBud)
		{
			_signalBus = signalBud;
			AfterInject();
		}

		public void LevelComplete()
		{
			_isStarted = false;
			EmitOnComplete();
		}

		public virtual void LevelStart()
		{
			_isStarted = true;
			EmitOnStart();
		}


		/// <summary>
		/// После иньекции
		/// </summary>
		protected virtual void AfterInject() { }
		/// <summary>
		/// После уничтожения
		/// </summary>
		protected virtual void AfterDispose() { }

		public void Dispose()
		{
			_OnComplete.RemoveAllListeners();
			_OnStart.RemoveAllListeners();
			AfterDispose();
		}

		public virtual void Tick() { }

		protected void EmitOnComplete()
		{
			_OnComplete?.Invoke();
		}

		protected void EmitOnStart()
		{
			_OnStart?.Invoke();
		}

		public IGameQuest SubscribeStart(UnityAction onComplete)
		{
			_OnStart.AddListener(onComplete);
			return this;
		}

		public IGameQuest UnsubscribeStart(UnityAction onComplete)
		{
			_OnStart.RemoveListener(onComplete);
			return this;
		}

		public IGameQuest SubscribeComplete(UnityAction onComplete)
		{
			_OnComplete.AddListener(onComplete);
			return this;
		}

		public IGameQuest UnsubscribeComplete(UnityAction onComplete)
		{
			_OnComplete.RemoveListener(onComplete);
			return this;
		}
	}
}
