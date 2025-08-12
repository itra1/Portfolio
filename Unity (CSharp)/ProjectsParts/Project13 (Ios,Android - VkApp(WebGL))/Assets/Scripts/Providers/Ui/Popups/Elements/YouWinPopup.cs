using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.Ui.Items;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using Game.Scenes;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.YouWin)]
	internal class YouWinPopup : Popup {

		protected IItemsAnimationParent _itemsAnimationParent;

		[Inject]
		public void InitSubComponents(IItemsAnimationParent itemParent) {
			_itemsAnimationParent = itemParent;
		}

		public override async UniTask Show() {
			await base.Show();

			System.Action disableAction = async () => {
				_itemsAnimationParent.ItemsAnimationPanel.SpawnItems(FlyingItemName.RoundPlayer, 1, _dialog, 1);
				await UniTask.Delay(500);
				_ = Hide();
			};

			disableAction();
		}

	}
}
