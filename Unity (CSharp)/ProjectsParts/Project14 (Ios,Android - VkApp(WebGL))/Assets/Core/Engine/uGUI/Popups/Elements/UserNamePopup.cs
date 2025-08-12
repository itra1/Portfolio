using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.User;

using TMPro;

using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Popups
{
	[PrefabName(PopupTypes.UserName)]
	public class UserNamePopup : Popup
	{
		[SerializeField] private TMP_InputField _userNameInput;

		private IUserProvider _userProvider;

		[Inject]
		public void Initiate(IUserProvider userProvider)
		{
			_userProvider = userProvider;
		}

		/// <summary>
		/// Подтверждение ввода
		/// </summary>
		public void EnternButton()
		{
			string targetname = _userNameInput.text;
			var result = _userProvider.EnterName(targetname);

			if (result)
				Hide();

		}
	}
}
