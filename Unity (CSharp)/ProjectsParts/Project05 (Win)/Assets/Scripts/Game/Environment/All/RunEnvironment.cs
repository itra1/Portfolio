using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Handles;
using UnityEngine.Events;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Environment
{

#if UNITY_EDITOR

	[CustomEditor(typeof(RunEnvironment))]
	public class EventEnvironmentEditor : EnvironmentEditor
	{
		SerializedProperty RunEvent, CompleteEvent, ClearEvent;

		//private RunEnvironment _script;

		private bool _eventsVisible;

		protected override void OnEnable()
		{
			base.OnEnable();
			//_script = (RunEnvironment)target;
			RunEvent = serializedObject.FindProperty("RunEvent");
			CompleteEvent = serializedObject.FindProperty("CompleteEvent");
			ClearEvent = serializedObject.FindProperty("ClearEvent");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Separator();

			if ((_eventsVisible = EditorGUILayout.Foldout(_eventsVisible, "Events")))
			{
				EditorGUILayout.PropertyField(RunEvent);
				EditorGUILayout.PropertyField(CompleteEvent);
				EditorGUILayout.PropertyField(ClearEvent);
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.Separator();

			if (GUILayout.Button("Create Collider"))
			{
				(_script as RunEnvironment).CreateCollider();
			}

		}
	}

#endif

	public class RunEnvironment : Environment
	{
		[SerializeField] [HideInInspector] private UnityEvent RunEvent;
		[SerializeField] [HideInInspector] private UnityEvent CompleteEvent;
		[SerializeField] [HideInInspector] private UnityEvent ClearEvent;

		[ContextMenu("Play")]
		public virtual void Play()
		{

			if (State > 0)
				return;

			IInteractionCondition condition = GetComponent<IInteractionCondition>();

			if (condition != null)
			{
				if (!condition.InteractionReady())
					return;
			}

			RunEvent?.Invoke();
			State = 1;
			Save();
		}

		protected override void ConfirmState(bool isForce = false)
		{
			base.ConfirmState(isForce);

			if (isForce)
			{
				if (State != 0)
					CompleteEvent?.Invoke();
				else
					ClearEvent?.Invoke();
			}
		}


#if UNITY_EDITOR
		[ContextMenu("Create collider")]
		public void CreateCollider()
		{
			GameObject obj = new GameObject();
			obj.name = "Trigger";
			obj.transform.SetParent(transform);
			obj.transform.localPosition = Vector3.zero;
			var collider = obj.AddComponent<BoxCollider>();
			collider.isTrigger = true;
			TriggerPlayerHandler trg = obj.AddComponent<TriggerPlayerHandler>();
			trg.onTriggerEnter = new UnityEngine.Events.UnityEvent();
		}

#endif
	}
}