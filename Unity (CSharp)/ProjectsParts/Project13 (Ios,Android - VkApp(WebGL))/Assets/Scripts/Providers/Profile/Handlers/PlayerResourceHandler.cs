using Game.Providers.Ui.Factorys;
using Game.Scenes;
using UnityEngine;

namespace Game.Providers.Profile.Handlers {
	public abstract class PlayerResourceHandler : IPlayerResourceHandler {

		protected UiPlayerResourcesFactory _uiPlayerResourcesFactory;
		protected IItemsAnimationParent _itemsAnimationParent;

		public abstract string ResourceType { get; }

		public abstract float CurrentValue { get; }

		public abstract string CurrentValueString { get; }

		protected abstract void SetValue(float value, RectTransform point);

		protected PlayerResourceHandler(UiPlayerResourcesFactory uiPlayerResourcesFactory, IItemsAnimationParent itemParent) {
			_uiPlayerResourcesFactory = uiPlayerResourcesFactory;
			_itemsAnimationParent = itemParent;
		}

		public bool AddValue(float value, RectTransform point) {
			if (value < 0 && -value > CurrentValue)
				return false;
			SetValue(CurrentValue + value, point);
			return true;
		}

		protected void AnimateMovePoint(string type, int increment, RectTransform point) {
			_itemsAnimationParent.ItemsAnimationPanel.SpawnItems(type, increment, point);
		}
	}
}
