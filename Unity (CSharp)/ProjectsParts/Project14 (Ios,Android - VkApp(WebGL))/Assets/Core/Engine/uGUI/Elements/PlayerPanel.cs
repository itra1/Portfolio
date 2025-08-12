using Core.Engine.Components.Avatars;
using Core.Engine.Components.User;

using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Core.Engine.uGUI.Elements
{
	public class PlayerPanel : MonoBehaviour, IZInjection
	{
		[SerializeField] private TMP_Text _userNameLabel;
		[SerializeField] private RawImage _image;

		private IUserProvider _userProvider;
		private IAvatarsProvider _avatarProvider;

		[Inject]
		public void Initiate(IUserProvider userProvider, IAvatarsProvider avatarProvider){
			_userProvider = userProvider;
			_avatarProvider = avatarProvider;
		}

		private void OnEnable()
		{
			_userNameLabel.text = _userProvider.UserName;
			if(_image != null) 
				_image.texture = _avatarProvider.GetTexture(_userProvider.AvatarName);
		}

	}
}