using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Handles;
using HutongGames.PlayMaker;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Environment
{

#if UNITY_EDITOR

	[CustomEditor(typeof(PlayMakerRun))]
	public class PlayMakerRunEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Create Collider"))
			{
				((PlayMakerRun)target).CreateCollider();
			}

		}
	}

#endif


	public class PlayMakerRun : Environment
	{
		[SerializeField] protected PlayMakerFSM _fsm;
		[SerializeField] protected string _eventRun = "StartFSM";
		[SerializeField] protected string _eventComplete = "CompleteFSM";
		[SerializeField] protected string _eventClear = "ClearFSM";

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

			_fsm.gameObject.SetActive(true);

			_fsm.SendEvent(_eventRun);
			State = 1;
			Save();
		}

		private void OnDrawGizmosSelected()
		{
			if (_fsm == null)
				_fsm = GetComponent<PlayMakerFSM>();
		}

		protected override void ConfirmState(bool isForce = false)
		{
			base.ConfirmState(isForce);

			if (isForce)
			{

				_fsm.SendEvent(_eventClear);

				if (State != 0)
				{
					_fsm.SendEvent(_eventComplete);
				}
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