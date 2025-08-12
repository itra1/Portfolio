using UnityEngine;
using UnityEngine.Events;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Engine.Components.Skins {
#if UNITY_EDITOR
	[CustomEditor(typeof(Skin), true)]
	public class SkinEditor : Editor {
		private Skin _target;
		private string _className;

		private void OnEnable() {
			_target = (Skin)target;
			_className = target.GetType().Name;
			string targetName = $"{_className}: {_target.Title}";

			if (!string.IsNullOrEmpty(targetName) && targetName != _target.gameObject.name)
				_target.gameObject.name = targetName;

		}

		public override void OnInspectorGUI() {
			GUILayout.Label($"Type: {_target.Type}");
			base.OnInspectorGUI();
		}

	}

#endif


	public abstract class Skin : MonoBehaviour, ISkin {
		[SerializeField] protected bool _isDefault;
		[SerializeField, Uuid.UUID] protected string _uuid;
		[SerializeField] protected string _title;
		[SerializeField, Multiline] protected string _description;
		[SerializeField] protected Sprite _icone;

		private UnityEvent _onChange = new();
		private ISkinProvider _skinProvider;
		protected DiContainer _container;
		private SignalBus _signlBus;

		public bool IsDefault => _isDefault;
		public abstract string Type { get; }
		public string Title => _title;
		public string Description => _description;
		public string UUID => _uuid;
		public Sprite Icone => _icone;

		public bool IsSelected => _skinProvider.IsActiveSkin(Type, UUID);

		public bool ReadyToSelect => _skinProvider.IsReadyToSelect(UUID);

		[Inject]
		public void Initiate(DiContainer container, SignalBus signalBus) {
			_container = container;
			_signlBus = signalBus;
		}

		public abstract bool Confirm();

		public void SubscribeChange(UnityAction action) {
			_onChange.AddListener(action);
		}

		public void UnSubscribeChange(UnityAction action) {
			_onChange.RemoveListener(action);
		}

		protected void EmitChange() {
			_onChange?.Invoke();
		}

		public void SetProvider(ISkinProvider skinProvider) {
			_skinProvider = skinProvider;
		}
	}
}
