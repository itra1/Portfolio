using Game.Base;
using Game.Providers.Avatars;
using Game.Providers.Profile;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Presenters.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class AvatarPanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private RawImage _avatarImage;
		[SerializeField] private AspectRatioFitter _avatarARF;

		private IProfileProvider _profileProvider;
		private IAvatarsProvider _avatarsProvider;
		private PopupProvider _popupProvider;
		private SignalBus _signalBus;

		[Inject]
		public void Constructor(
			SignalBus signalBus,
			IProfileProvider profileProvider,
			IAvatarsProvider avatarsProvider,
			PopupProvider popupProvider
		)
		{
			_profileProvider = profileProvider;
			_avatarsProvider = avatarsProvider;
			_popupProvider = popupProvider;
			_signalBus = signalBus;

			_signalBus.Subscribe<AvatarChangeSignal>(SetData);
		}

		public void Show()
		{
			SetData();
		}

		public void Hide()
		{
		}

		private void SetData()
		{
			_avatarImage.texture = _avatarsProvider.GetTexture(_profileProvider.Avatar);
			_avatarARF.aspectRatio = _avatarImage.texture.width / (float) _avatarImage.texture.height;
		}
	}
}
