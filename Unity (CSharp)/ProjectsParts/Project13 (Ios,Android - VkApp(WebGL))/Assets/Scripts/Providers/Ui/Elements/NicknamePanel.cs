using Game.Base;
using Game.Providers.Profile;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class NicknamePanel : MonoBehaviour, IInjection
	{
		[SerializeField] private TMP_InputField _inputField;

		private IProfileProvider _profileProvider;

		[Inject]
		public void Constructor(IProfileProvider profileProvider)
		{
			_profileProvider = profileProvider;
		}

		public void Awake()
		{
			_inputField.onEndEdit.AddListener((data) =>
			{
				SetProfileName();
			});
			_inputField.onSubmit.AddListener((data) =>
			{
				if (data.Length <= 3)
				{
					_inputField.text = _profileProvider.Name;
					SetProfileName();
				}
				else
				{
					_profileProvider.SetNickname(data);
				}
			});
		}

		public void OnEnable()
		{
			SetProfileName();
		}

		private void SetProfileName()
		{
			_inputField.text = _profileProvider.Name.Trim();
		}

	}
}
