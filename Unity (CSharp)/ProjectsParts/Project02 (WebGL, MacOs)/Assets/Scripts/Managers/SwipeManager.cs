using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Garilla
{
	public interface ISwipe
	{
		public SwipeToClose SwipeType { get; }
		public UnityEngine.Events.UnityAction<SwipeToClose> OnSwipeEvent { get; }
	}

	public static class SwipeManager
	{
		public static List<ISwipe> SwipeStack;

		public static void Init()
		{
			SwipeStack = new List<ISwipe>();
			GameObject listener = new GameObject("_SwipeListener");
			listener.AddComponent<SwipeListener>();
		}

		public static void AddListener(ISwipe swiper)
		{
			if (SwipeStack == null)
				Init();
			if (!SwipeStack.Contains(swiper))
				SwipeStack.Add(swiper);
		}

		public static void RemoveListener(ISwipe swiper)
		{
			if (SwipeStack == null)
				Init();
			SwipeStack.Remove(swiper);
		}
	}
	[System.Flags]
	public enum SwipeToClose
	{
		None = 0,
		Left = 1,
		Right = 2,
		Lock = 4
	}

	public class SwipeListener : MonoBehaviour
	{
		private bool _isDown = false;
		private Vector2 _pointPosition;
		public static bool Lock { get; set; } = false;

		private void Update()
		{
			if (SwipeManager.SwipeStack == null || SwipeManager.SwipeStack.Count == 0) return;

			var elem = SwipeManager.SwipeStack.Last();

			if (elem.SwipeType != SwipeToClose.None)
			{
				if (!_isDown
#if (UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			&& Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
#else
	&& Input.GetMouseButtonDown(0)
#endif
				)
				{
					if (Lock) return;
					_isDown = true;
#if (UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
				var p1 = Input.GetTouch(0);
				_pointPosition = p1.position;
#else
					_pointPosition = Input.mousePosition;
#endif
				}
				if (_isDown
#if (UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
				&& Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended
#else
	&& (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
#endif
				)
				{
					if (Lock) return;
					_isDown = false;
#if (UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
				var p1 = Input.GetTouch(0);
					Vector2 delta = _pointPosition - p1.position;
#else
					Vector2 delta = _pointPosition - (Vector2)Input.mousePosition;
#endif
					if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
					{
						if ((elem.SwipeType & SwipeToClose.Left) != 0 && delta.x > 0 && Mathf.Abs(delta.x) > Screen.width / 5)
						{
							elem.OnSwipeEvent(SwipeToClose.Left);
						}
						if ((elem.SwipeType & SwipeToClose.Right) != 0 && delta.x < 0 && Mathf.Abs(delta.x) > Screen.width / 5)
						{
							elem.OnSwipeEvent(SwipeToClose.Right);
						}
					}
				}
			}
		}
	}
}