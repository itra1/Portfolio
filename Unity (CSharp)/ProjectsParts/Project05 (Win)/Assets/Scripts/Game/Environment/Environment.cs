using System.Collections;
using System.Collections.Generic;
using com.ootii.Messages;
using it.Game.Player.Save;
using UnityEngine;
using Leguar.TotalJSON;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Game.Environment
{


#if UNITY_EDITOR

	[CustomEditor(typeof(Environment), editorForChildClasses: true)]
	public class EnvironmentEditor : Editor
	{
		//SerializedProperty RunEvent, CompleteEvent, ClearEvent;
		SerializedProperty Title, Discription;

		protected Environment _script;
		private bool _info;

		protected virtual void OnEnable()
		{
			_script = (Environment)target;
			Title = serializedObject.FindProperty("_title");
			Discription = serializedObject.FindProperty("_description");
		}

		public override void OnInspectorGUI()
		{

			if ((_info = EditorGUILayout.Foldout(_info, "Info")))
			{
				EditorGUILayout.PropertyField(Title);
				EditorGUILayout.LabelField("Description:");
				Discription.stringValue = EditorGUILayout.TextArea(Discription.stringValue, GUILayout.Height(100));
				serializedObject.ApplyModifiedProperties();
			}

			EditorGUILayout.Separator();

			base.OnInspectorGUI();

		}
	}

#endif


	public class StateChangeEvent : UnityEngine.Events.UnityEvent<int> { };

	/// <summary>
	/// Базовый класс для объектов на сцене
	/// </summary>
	public abstract class Environment : UUIDBase, Game.Player.Save.ISave
	{
		[SerializeField] protected bool _isActived = true;
		/// <summary>
		/// Разрешить сохранение
		/// </summary>
		[SerializeField] private bool _isSaved = true;
		[SerializeField] [HideInInspector] private string _title;
		[SerializeField] [HideInInspector] private string _description;
		[SerializeField] protected bool _isDebug;
		protected bool IsSaved { get => _isSaved; set => _isSaved = value; }

		public string Title { get => _title; set => _title = value; }
		public string Discription { get => _description; set => _description = value; }

		/// <summary>
		/// Происходит катсцена
		/// </summary>
		public bool IsCutScene { get; set; }
		public StateChangeEvent OnStateChangeEvent = new StateChangeEvent();
		private int _state = 0;
		protected bool _stateChange;
		protected virtual int InitState => 0;

		public int State
		{
			get => _state;
			protected set
			{
				_state = value;
				OnStateChange();
			}
		}

		protected virtual void OnStateChange()
		{
			OnStateChangeEvent?.Invoke(_state);
		}

		protected virtual void Awake()
		{
#if !UNITY_EDITOR
		_isActived = true;
#endif
		}

		protected virtual void Start()
		{
			Load(Game.Managers.GameManager.Instance.UserManager.PlayerProgress);
			SubscribeSaveEvents();
			State = InitState;
			ConfirmState();
		}

		private void OnDrawGizmosSelected()
		{
			if (!string.IsNullOrEmpty(_title))
			{
				string t = "Env: " + _title;
				if (!gameObject.name.Equals(t))
					gameObject.name = t;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "StarIcon");
		}

		protected virtual void OnDestroy()
		{
			UnsubscribeSaveEvents();
		}

		protected virtual void ConfirmState(bool isForce = false) { }

		protected virtual void NoLoadData()
		{
			State = InitState;
		}

		#region Save
		public void Load(PlayerProgress progress)
		{
			if (!progress.IsLoad)
				return;
			BeforeLoad();

			JValue data = progress.GetEnvironment(Uuid);

			if (data == null)
			{
				NoLoadData();
				ConfirmState(true);
				return;
			}

			JSON loadData = data as JSON;

			int newState = loadData.GetInt("state");

			_stateChange = newState != State;
			State = newState;
			LoadData(loadData.Get("data"));
			ConfirmState(true);
			AfterLoad();
		}

		protected virtual void BeforeLoad() { }
		protected virtual void AfterLoad() { }
		protected virtual void LoadData(JValue data) { }

		public void LoadHandler(IMessage handler)
		{
			Load((handler as Game.Events.Messages.LoadData).SaveData);
		}

		public virtual void SubscribeSaveEvents()
		{
			Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
		}

		public virtual void UnsubscribeSaveEvents()
		{
			Game.Events.EventDispatcher.RemoveListener(EventsConstants.PlayerProgressLoad, LoadHandler);
		}

		public void Save()
		{
			if (!_isSaved) return;

			JSON saveData = new JSON();
			saveData.Add("state", State);
			saveData.Add("data", SaveData());
			SendSaveMessage(saveData);
		}

		protected virtual JValue SaveData()
		{
			return new JSON();
		}

		private void SendSaveMessage(JValue data)
		{

			Game.Events.Messages.SaveData saveEvent = Events.Messages.SaveData.Allocate();
			saveEvent.Type = EventsConstants.PlayerProgressSave;
			saveEvent.Sender = this;
			saveEvent.Entity = Events.Messages.SaveData.EntityType.environment;
			saveEvent.Environment.Add(Uuid, data);

			Game.Events.EventDispatcher.SendMessage(saveEvent);
		}

		#endregion

#if UNITY_EDITOR

		[ContextMenu("Reset save")]
		private void ResetSave()
		{
			State = InitState;
			Save();
		}


#endif


	}
}