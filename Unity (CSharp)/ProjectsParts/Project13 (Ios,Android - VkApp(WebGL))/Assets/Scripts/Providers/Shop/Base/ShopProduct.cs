using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Uuid;
using Game.Providers.Profile;
using Game.Providers.Saves;


#if UNITY_EDITOR

using UnityEditor;
#endif

namespace Game.Providers.Shop
{
#if UNITY_EDITOR
	[CustomEditor(typeof(ShopProduct), true)]
	public class ShopItemEditor : Editor
	{
		private ShopProduct _target;

		private void OnEnable()
		{
			_target = (ShopProduct) target;
			var targetName = $"Shop product: {_target.Title}";

			if (!string.IsNullOrEmpty(targetName) && targetName != _target.gameObject.name)
			{
				_target.gameObject.name = targetName;
			}
		}

		public override void OnInspectorGUI()
		{
			GUILayout.Label($"Group type: {_target.GroupType}");
			base.OnInspectorGUI();
		}
	}

#endif

	public abstract class ShopProduct : MonoBehaviour, IShopProduct
	{
		[SerializeField, UUID] private string _uuid;
		[SerializeField] protected PlatformSystemPlatformType _platform;
		[SerializeField] private string _title;
		[SerializeField, Multiline] private string _description;
		[SerializeField] private bool _isVisibleShop = true;
		[SerializeField] private ulong _price;
		[SerializeField] private Sprite _icone;

		private IProfileProvider _profileProvider;
		private ShopSave _shopSave;
		private UnityEvent _onChange = new();

		public string UUID => _uuid;
		public PlatformSystemPlatformType Platform => _platform;
		public string Title => _title;
		public string Description => _description;
		public ulong Price
		{
			get
			{
#if UNITY_EDITOR && SHOP_FREE_SYMBOLS
				return 0;
#else
				return _price;
#endif
			}
		}
		public bool IsVisibleShop => _isVisibleShop;
		public virtual bool IsBuyReady
		{
			get
			{
#if UNITY_EDITOR && SHOP_FREE_SYMBOLS
				return true;
#else
				return false; // _profileProvider. >= Price;
#endif
			}
		}
		public abstract bool IsAlreadyBuyed { get; }
		public Sprite Icone => _icone;
		public abstract string GroupType { get; }

		[Inject]
		public void Initialize(IProfileProvider profileProvider, ISaveProvider sg)
		{
			_profileProvider = profileProvider;
			_shopSave = (ShopSave) sg.GetProperty<ShopSave>();
		}

		public bool Buy()
		{
			if (!IsBuyReady && !IsAlreadyBuyed)
				return false;

			//_profileProvider.PointsSubtract(Price);
			ConfirmProduct();
			AddToSave();

			return true;
		}

		protected abstract void ConfirmProduct();

		private void AddToSave()
		{
			if (IsAlreadyBuyed)
				return;
			_shopSave.Value.BuyedProducts.Add(UUID);
			_ = _shopSave.Save();
		}

		public void SubscribeChange(UnityAction action)
		{
			_onChange.AddListener(action);
		}

		public void UnSubscribeChange(UnityAction action)
		{
			_onChange.RemoveListener(action);
		}
	}
}
