using it.Network.Rest;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace it.Popups
{
	public class CashierCryptoDetailsForm : CashierDetailsForm
	{
		[SerializeField] private RectTransform _body;
		[SerializeField] private RawImage _qrImage;
		[SerializeField] private ScrollRect _cryptoScrollRect;
		[SerializeField] private TextMeshProUGUI _cryptoWalletLabel;

		protected override void OnEnable()
		{
			base.OnEnable();
			_body.gameObject.SetActive(false);
			if (_cryptoWalletLabel != null)
				_cryptoWalletLabel.text = "";
		}
		public void CopyToClipboardCryptoWalletTouch()
		{
#if UNITY_WEBGL && !UNITY_EDITOR

			Garilla.Platform.WebGL.WebGLCopyPastle.CopyText(_cryptoWalletLabel.text);

#else
			if (_cryptoWalletLabel != null)
				UniClipboard.SetText(_cryptoWalletLabel.text);
#endif


			it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info)
			.SetDescriptionString("message.info.copy".Localized())
			.SetTimer(0.5f);


			//it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}

		public override void Set(SelectMethod method)
		{
			method.IsQR = true;
			base.Set(method);
			ProcessDepositeRequest();
		}

		protected override void ProcessDepositeRequest()
		{
			var replenishment = GetReplenishmentDeposite();
			replenishment.amount = 100;
			RequestDeposite(replenishment);
		}

		protected override void ParceQR(string input)
		{
			try
			{
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<CashierDepositeResponse>(input).data;
				if (_cryptoWalletLabel != null)
				{
					_cryptoWalletLabel.text = data.addr;
					if (_cryptoScrollRect != null){
						_cryptoScrollRect.content.sizeDelta = new Vector2(_cryptoWalletLabel.preferredWidth, _cryptoScrollRect.content.sizeDelta.y);
					}

				}
				if (_qrImage != null)
				{
					var bytes = System.Convert.FromBase64String(data.qr);
					Texture2D tex = new Texture2D(4, 4, TextureFormat.RGB24, false);
					tex.LoadImage(bytes);
					tex.Apply();
					_qrImage.texture = tex;
				}

				_body.gameObject.SetActive(true);
			}
			catch
			{
				it.Main.PopupController.Instance.ShowPopup<InfoPopup>(PopupType.Info).SetDescriptionString("errors.forms.anyError".Localized());
				it.Logger.Log("Ошибка обработки ответа qr");
			}
		}

	}
}