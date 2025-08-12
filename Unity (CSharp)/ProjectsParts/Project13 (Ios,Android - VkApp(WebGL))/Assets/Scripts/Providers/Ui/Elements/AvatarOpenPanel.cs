using Cysharp.Threading.Tasks;
using Game.Base;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	[RequireComponent(typeof(Button))]
	public class AvatarOpenPanel : MonoBehaviour, IInjection
	{
		private PopupProvider _popupProvider;

		[Inject]
		public void Constructor(PopupProvider popupProvider)
		{
			_popupProvider = popupProvider;
		}

		public void Awake()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				_popupProvider.GetPopup(PopupsNames.Avatars).Show().Forget();
			});
		}

	}
}
