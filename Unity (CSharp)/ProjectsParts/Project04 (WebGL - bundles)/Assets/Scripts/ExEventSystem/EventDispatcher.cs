using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExEvent {
	public class EventDispatcher : MonoBehaviour {
		// Turn on logs in Runtime, inspector
		public bool LoggerEnabled = false;
		private static EventDispatcher _instance;

		private Dictionary<Type, List<MessageHandlerData>> handlerHash = new Dictionary<Type, List<MessageHandlerData>>();
		private List<MessageHandlerData> m_removedList = new List<MessageHandlerData>();
		private Queue<BaseEvent> asyncEventsQueue = new Queue<BaseEvent>();
		private float lastAsyncQueuePopulate = 0f;
		private bool invokeRunning = false;

		private class MessageHandlerData {
			public object Container;
			public MethodInfo Method;
			public bool OnlyIfEnabled;
		}

		public static EventDispatcher Instance {
			get {
				if (_instance == null) {
					var go = new GameObject("!ExEventDispatcher", typeof(EventDispatcher));
					go.transform.SetAsFirstSibling();
					_instance = go.GetComponent<EventDispatcher>();
				}

				return _instance;
			}
		}

		private EventDispatcher() { }

		public void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		public void RegisterMessageHandler(Type eventType, bool onlyIfEnabled, object container, MethodInfo methodInfo) {
			var methodName = methodInfo.Name;
			if (!handlerHash.ContainsKey(eventType)) {
				handlerHash.Add(eventType, new List<MessageHandlerData>());
			}

			var messageHanlders = (List<MessageHandlerData>)handlerHash[eventType];

			messageHanlders.Add(new MessageHandlerData() { Container = container, Method = methodInfo, OnlyIfEnabled = onlyIfEnabled });
		}


		public void UnregisterMessageHandler(Type eventType, object container, MethodInfo methodInfo) {
			var methodName = methodInfo.Name;
			if (handlerHash.ContainsKey(eventType)) {
				var messageHanlders = (List<MessageHandlerData>)handlerHash[eventType];
				for (var i = 0; i < messageHanlders.Count; i++) {
					var mhd = messageHanlders[i];

					if (mhd.Container == container && mhd.Method == methodInfo) {
						messageHanlders.Remove(mhd);
						return;
					}
				}
			}
		}

		public void CallAsync(BaseEvent e) {
			if (e != null) {
				lastAsyncQueuePopulate = Time.timeSinceLevelLoad;
				asyncEventsQueue.Enqueue(e);
			}
		}

		public void Call(BaseEvent e, bool isAsync = false) {
			var eventType = e.GetType();
			if (handlerHash.ContainsKey(eventType)) {
				var parameters = new object[] { e };
				var hanlderList = (List<MessageHandlerData>)handlerHash[eventType];

				for (var i = 0; i < hanlderList.Count; i++) {
					var mhd = hanlderList[i];
					var unityObject = (MonoBehaviour)mhd.Container;

					if (unityObject != null) {
						if (!mhd.OnlyIfEnabled || unityObject.gameObject.activeSelf) {
							try {
								if (LoggerEnabled)
									Debug.Log("Event " + e.GetType().ToString() + " Fired to " + mhd.Container.ToString() + " async: " + isAsync);
								mhd.Method.Invoke(mhd.Container, parameters);
							} catch (Exception ex) {
								Debug.LogError("Exception while performing Event :" + e.GetType() + " async: " + isAsync);
								Debug.LogException(ex);
							}
						}
					} else {
						m_removedList.Add(mhd);
					}
				}

				for (var i = 0; i < m_removedList.Count; i++) {
					hanlderList.Remove(m_removedList[i]);
				}

				m_removedList.Clear();
			}
		}

		void LateUpdate() {
			if (asyncEventsQueue.Count > 0) {
				StartCoroutine(InvokeNextFrame(lastAsyncQueuePopulate, asyncEventsQueue));
				asyncEventsQueue = new Queue<BaseEvent>();
			}
		}

		private IEnumerator InvokeNextFrame(float lastPopulate, Queue<BaseEvent> lastQueue) {
			if (invokeRunning)
				yield break;
			invokeRunning = true;

			// Be sure to do really async ops (separate frames)
			if (Time.timeSinceLevelLoad == lastPopulate)
				yield return null;

			while (lastQueue.Count > 0) {
				var evt = lastQueue.Dequeue();
				Call(evt, true);
			}

			invokeRunning = false;
		}

		public void OnDestroy() {
			foreach (var handlers in handlerHash.Values) {
				var messageHanlders = (List<MessageHandlerData>)handlers;

				messageHanlders.Clear();
			}

			handlerHash.Clear();
			handlerHash = null;
		}
	}
}