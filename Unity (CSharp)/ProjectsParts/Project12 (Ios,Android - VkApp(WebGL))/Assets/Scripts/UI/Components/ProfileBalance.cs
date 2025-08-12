using Engine.Scripts.Base;
using Game.Scripts.Providers.Profiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class ProfileBalance : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_Text _balanceLabel;
		[SerializeField] private Button _openButton;

		private IProfileProvider _profileProvider;

		[Inject]
		private void Build(IProfileProvider profileProvider)
		{
			_profileProvider = profileProvider;

			_openButton.onClick.RemoveAllListeners();
			_openButton.onClick.AddListener(SelfButtonTouch);

			SetData();
		}

		public void SelfButtonTouch()
		{
		}

		private void SetData()
		{
			_balanceLabel.text = _profileProvider.Profile.Balance.ToString("### ### ##0");
		}
	}
}
