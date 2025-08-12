using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Garilla.Games.Animations
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TableAnimationAttribute : System.Attribute
	{
		public TableAnimationsType Type;
		public bool InOrderExecute;
		public bool WaitComplete;

		public TableAnimationAttribute(TableAnimationsType type, bool inOrderExecute = true, bool waitComplete = true)
		{
			this.Type = type;
			InOrderExecute = inOrderExecute;
			WaitComplete = waitComplete;
		}

	}

	public class TableAnimationsManager
	{
		private const string _logMaker = "[ANIM]";

		private List<ulong> _processEvents = new List<ulong>();
		public it.UI.GamePanel BasePanel;

		private Dictionary<TableAnimationsType, Type> _animations;
		private Queue<QueueItem> _animationsQueue = new Queue<QueueItem>();
		private GameAnimationsBase _activeAnimation;

		public class QueueItem
		{
			public TableAnimationsType Type;
			public GameAnimationsBase Class;
			public Hashtable Hash;
			public UnityEngine.Events.UnityAction OnComplete;
		}


		public bool IsActiveAnimation => _activeAnimation != null;

		public List<ulong> ProcessEvents { get => _processEvents; set => _processEvents = value; }

		public TableAnimationsManager()
		{
			FindAnimations();
		}

		public void FindAnimations()
		{
			_animations = new Dictionary<TableAnimationsType, Type>();

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(GameAnimationsBase))).ToArray();

			for (int i = 0; i < types.Length; i++)
			{
				object[] ob = types[i].GetCustomAttributes(false);
				for (int x = 0; x < ob.Length; x++)
				{
					if (ob[x].GetType() == typeof(TableAnimationAttribute))
						_animations.Add((ob[x] as TableAnimationAttribute).Type, types[i]);
				}
			}
		}

		public void AddAnimation(ulong eventId, TableAnimationsType animationType, Hashtable hash, UnityEngine.Events.UnityAction OnComplete = null)
		{
			if (eventId != ulong.MaxValue)
			{
				if (ProcessEvents.Contains(eventId)) return;
				ProcessEvents.Add(eventId);
			}
			it.Logger.Log($"{_logMaker} Add animation " + animationType.ToString());

			var enim = (GameAnimationsBase)System.Activator.CreateInstance(_animations[animationType]);

			object[] attrs = enim.GetType().GetCustomAttributes(typeof(TableAnimationAttribute), false);
			bool isOrderExec = (attrs[0] as TableAnimationAttribute).InOrderExecute;

			enim.AnimationManager = this;
			enim.Init(hash);

			if (!isOrderExec)
			{
				it.Logger.Log($"{_logMaker} Play animation " + (attrs[0] as TableAnimationAttribute).Type.ToString());
				enim.OnComplete = () =>
				{
					OnComplete?.Invoke();
				};
				enim.Play();
				return;
			}

			_animationsQueue.Enqueue(new QueueItem() { Class = enim, Type = (attrs[0] as TableAnimationAttribute).Type, Hash = hash, OnComplete = OnComplete });
			NextAnimation();
		}

		private void NextAnimation()
		{
			if (IsActiveAnimation) return;

			if (_animationsQueue.Count <= 0) return;

			var queueItem = _animationsQueue.Dequeue();

			_activeAnimation = queueItem.Class;

			object[] attrs = _activeAnimation.GetType().GetCustomAttributes(typeof(TableAnimationAttribute), false);

			bool waitComplete = (attrs[0] as TableAnimationAttribute).WaitComplete;

			if (waitComplete)
				_activeAnimation.OnComplete = () =>
				{
					queueItem.OnComplete?.Invoke();
					AnimationComnplete();
				};

			it.Logger.Log($"{_logMaker} Play animation " + (attrs[0] as TableAnimationAttribute).Type.ToString());
			try
			{
				_activeAnimation.Play();
			}
			catch(System.Exception ex)
			{
				it.Logger.LogError($"{_logMaker} Play animation ERROR" + ex.Message);
			}
			if (!waitComplete)
			{
				queueItem.OnComplete?.Invoke();
				AnimationComnplete();
			}

		}

		private void AnimationComnplete()
		{
			_activeAnimation = null;
			NextAnimation();
		}

	}
}