using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace it.UI
{
	public class SmilePresets : MonoBehaviour
	{
		[SerializeField] private RawImage[] _images;
		[SerializeField] private RectTransform _content;
		[SerializeField] private bool _isFixed = false;


		private Coroutine _disableAnimation;
		private int _indexPreset;
		private ChatController _chatManager;

		private void OnEnable()
		{
			var sPresets = UserController.ReferenceData.smile_presets[0];
			_indexPreset = UnityEngine.Random.Range(0, sPresets.smiles.Length);

			for (int i = 0; i < _images.Length; i++)
			{
				int index = i;
				_images[index].gameObject.SetActive(false);

				it.UI.Elements.GraphicButtonUI _btn = _images[index].transform.parent.GetComponent<it.UI.Elements.GraphicButtonUI>();

				_btn.OnClick.RemoveAllListeners();
				_btn.OnClick.AddListener(() =>
				{
					ButtonTouch((int)sPresets.smiles[_indexPreset][index].id);
				});

				it.Managers.NetworkManager.Instance.RequestTexture(sPresets.smiles[_indexPreset][index].url, (s, b) =>
				{
					_images[index].gameObject.SetActive(true);
					_images[index].texture = s;
					_images[index].GetComponent<AspectRatioFitter>().aspectRatio = (float)s.width / (float)s.height;
				}, (err) => { });

			}
			_content.transform.localScale = Vector3.zero;
			_content.DOScale(Vector3.one, 0.3f).OnComplete(() =>
			{
				_disableAnimation = StartCoroutine(DisableCor());
				if (_isFixed) return;
				//_content.SetParent(GetComponentInParent<GamePanel>(true).transform);
				_content.SetAsLastSibling();
			});
		}

		IEnumerator DisableCor()
		{
			yield return new WaitForSeconds(2);
			Hide();

		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		public void SetData(ChatController manager)
		{
			_chatManager = manager;

		}

		public void ButtonTouch(int smileId)
		{
			if (_disableAnimation != null)
				StopCoroutine(_disableAnimation);
			_chatManager.UseSmile(smileId);

			Hide();
		}

		private void Hide()
		{
			for (int i = 0; i < _images.Length; i++)
			{
				it.UI.Elements.GraphicButtonUI _btn = _images[i].transform.parent.GetComponent<it.UI.Elements.GraphicButtonUI>();
				_btn.OnClick.RemoveAllListeners();
			}

			_content.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
			{
				if (_isFixed)
				{
					gameObject.SetActive(false);
					return;
				}
				_content.transform.SetParent(transform);
				Destroy(gameObject);
			});
		}

	}
}