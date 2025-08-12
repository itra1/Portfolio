using System.Collections;
using it.UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;
using UnityEngine.UIElements;
using it.UI;
using it.Popups;

namespace Garilla.Main
{
	/// <summary>
	/// Каруселька бросаемых смайлов
	/// </summary>
	public class DropSmilesCarusel : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;

		private it.UI.GamePanel _gamePanel;
		private ScrollRect _scroll;
		private PoolList<DropSmileItem> _itemPooler;
		private ulong _userId;

		public GamePanel GamePanel { get => _gamePanel; set => _gamePanel = value; }
		public ulong UserId { get => _userId; set => _userId = value; }

		private bool _isSend = false;

		private void Awake()
		{
			_scroll = GetComponent<ScrollRect>();
			var itm = _prefab.AddComponent<DropSmileItem>();
			_itemPooler = new PoolList<DropSmileItem>(itm, _scroll.content);
			_prefab.gameObject.SetActive(false);

			_itemPooler.HideAll();

			ReadData();
		}

		private void OnEnable()
		{
			_isSend = false;
		}

		public void LockSwipe(bool isLock){
			Garilla.SwipeListener.Lock = isLock;
		}

		private void ReadData()
		{

			var sendSmilea = UserController.ReferenceData.smiles_to_send;
			var hlg = _scroll.content.GetComponent<HorizontalLayoutGroup>();
			var pRect = _prefab.GetComponent<RectTransform>();

			_scroll.content.sizeDelta = new Vector2(hlg.padding.left + hlg.padding.right + (sendSmilea.Length * pRect.rect.width) + ((sendSmilea.Length - 1) * hlg.spacing), _scroll.content.sizeDelta.y);

			for (int i = 0; i < sendSmilea.Length; i++)
			{
				int index = i;
				var sm = sendSmilea[i];
				it.Managers.NetworkManager.Instance.RequestTexture(sm.url, (t, b) =>
				{
					var item = _itemPooler.GetItem();
					item.SetData(sendSmilea[index], t);
					item.transform.SetSiblingIndex(index);
					item._button.OnClick.RemoveAllListeners();
					item._button.OnClick.AddListener(() =>
					{
						if (_isSend) return;

						_isSend = true;

						var pb = GetComponentInParent<PopupBase>();
						if (pb != null)
							pb.Hide(()=> {
								_gamePanel.ChatManager.DropSmileActive(UserController.User.id, _userId, (ulong)sm.id);
							});

						_gamePanel.ChatManager.DropSmileSend(_userId, (ulong)sm.id, null);
					});
				}, null);

			}
		}

	}

	public class DropSmileItem : MonoBehaviour
	{

		private RawImage _image;
		public GraphicButtonUI _button;
		private Smile _smile;
		private Texture _texture;

		private void ReadComponents()
		{
			_button = GetComponent<GraphicButtonUI>();
			_image = GetComponentInChildren<RawImage>();
		}

		public void SetData(Smile smile, Texture t)
		{
			_smile = smile;
			_texture = t;

			if (_image == null)
				ReadComponents();
			_image.texture = _texture;
		}
	}
}