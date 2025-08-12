using System;

using UnityEngine;
using UnityEngine.UI;

namespace it.UI
{
	public class Avatar : MonoBehaviour
	{
		[SerializeField] private RawImage _avatar;

		//[SerializeField] private Image _defaultAvatar;

		public string Url { get; set; } = "";
		private bool _existsAvatar;

		public void SetDefaultAvatar()
		{
			//Url = "";
			if (_avatar == null)
			{
				it.Logger.Log("SetDefaultAvatar avatar");
				return;
			}

			_avatar.gameObject.SetActive(false);
			//	_defaultAvatar.gameObject.SetActive(true);
		}

		private void VisibleAvatar()
		{

		}

		public void SetAvatar(string url)
		{
			if (Url == url)
			{
				if (_existsAvatar)
					_avatar.gameObject.SetActive(true);
			};
			if (string.IsNullOrEmpty(url))
			{
				SetDefaultAvatar();
				return;
			}

			Url = string.Copy(url);
			_existsAvatar = false;
			it.Managers.NetworkManager.Instance.RequestTexture(Url, (s, b) =>
			{
				if (_avatar == null) return;
				_existsAvatar = true;
				_avatar.texture = s;
				_avatar.gameObject.SetActive(true);
				_avatar.GetComponent<AspectRatioFitter>().aspectRatio = (float)s.width / (float)s.height;
				//_defaultAvatar.gameObject.SetActive(false);

			}, (err) =>
			{
				SetDefaultAvatar();
			});
		}

	}
}