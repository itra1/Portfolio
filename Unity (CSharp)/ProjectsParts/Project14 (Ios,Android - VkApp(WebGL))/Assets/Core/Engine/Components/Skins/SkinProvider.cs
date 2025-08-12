using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Core.Engine.Components.SaveGame;
using Zenject;

namespace Core.Engine.Components.Skins
{
	/// <summary>
	/// Провайдер скинов
	/// </summary>
	public class SkinProvider : ISkinProvider
	{
		private readonly ISkinSettings _skinSettings;
		private readonly SkinSave _saveData;
		private List<ISkin> _items = new();
		private SignalBus _singalBus;
		private DiContainer _diContainer;

		public List<ISkin> Items => _items;

		public SkinProvider(DiContainer diContainer
		, ISkinSettings skinSettings
		, SaveGameProvider saveDataProvider
		, SignalBus signalBus)
		{
			_skinSettings = skinSettings;
			_diContainer = diContainer;
			_singalBus = signalBus;
			FindItems();
			_saveData = (SkinSave)saveDataProvider.GetProperty<SkinSave>();
		}
		private void FindItems()
		{
			var items = Resources.LoadAll<Skin>(_skinSettings.SkinFolder).ToList();

			foreach (var item in items)
			{
				if (item.TryGetComponent<ISkin>(out var itm))
				{
					_diContainer.Inject(itm);
					itm.SetProvider(this);
					_items.Add(itm);
				}
			}

			AppLog.Log($"Skins count {_items.Count}");
		}

		public bool IsActiveSkin(string skinType, string uuid)
		{
			return GetActiveSkin(skinType).UUID == uuid;
		}

		public ISkin GetActiveSkin(string skinType)
		{
			if (!_saveData.Value.ActiveSkins.ContainsKey(skinType))
			{
				_saveData.Value.ActiveSkins.Add(skinType, GetDefaultSkin(skinType).UUID);
				Save();
			}
			return _items.Find(x => x.UUID == _saveData.Value.ActiveSkins[skinType]);
		}

		private ISkin GetDefaultSkin(string skinType)
		{
			var defSkin = _items.Find(x => x.Type == skinType && x.IsDefault);

			if (defSkin == null)
				defSkin = _items.FindAll(x => x.Type == skinType)[0];

			return defSkin;
		}

		public void SetActiveSkin(ISkin skin)
		{
			SetActiveSkin(skin.Type, skin.UUID);
		}

		public void SetActiveSkin(string skinType, string uuid)
		{
			if (!_saveData.Value.ActiveSkins.ContainsKey(skinType))
			{
				_saveData.Value.ActiveSkins.Add(skinType, uuid);
			}
			else
			{
				_saveData.Value.ActiveSkins[skinType] = uuid;
			}
			Save();
			_singalBus.Fire<SetSkinSignal>();
		}

		private void Save()
		{
			_saveData.Save();
		}

		public void AddReadySkin(string uuid)
		{
			if (!IsReadyToSelect(uuid))
				_saveData.Value.ReadySkins.Add(uuid);
			Save();
		}

		public bool IsReadyToSelect(string uuid)
		{
			return _saveData.Value.ReadySkins.Contains(uuid);
		}
	}
}
