using UnityEngine;
using UnityEngine.UI;

namespace it.Popups
{
	public class QrPopup : PopupBase
	{
		[SerializeField] private RawImage _qrImage;

		public void SetBase64String(string data){

			var bytes = System.Convert.FromBase64String(data);
			Texture2D tex = new Texture2D(4, 4, TextureFormat.RGB24, false);
			tex.LoadImage(bytes);
			tex.Apply();
			_qrImage.texture = tex;
		}

		public void CloseTouch(){
			Hide();
		}

	}
}