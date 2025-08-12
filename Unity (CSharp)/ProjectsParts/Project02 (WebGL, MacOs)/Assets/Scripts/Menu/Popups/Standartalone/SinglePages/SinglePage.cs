using DG.Tweening;
using it.Popups;
using it.UI.Profiles;
using System.Collections;
using UnityEngine;

namespace it.Main.SinglePages
{
	public class SinglePage : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnVisibleComplete;
		[HideInInspector] public UnityEngine.Events.UnityEvent OnClose;

		public SinglePagesType PageType;

		private void OnEnable()
		{
			var btn = gameObject.GetComponentInChildren<CloseButton>();
			if (btn != null)
			{
				var ibtn = btn.GetComponent<IButton>();
				ibtn.OnClickPointer.RemoveAllListeners();
				ibtn.OnClickPointer.AddListener(() =>
				{
					Hide();
				});
			}

			EnableInit();
			Localize();

		}
		protected virtual void EnableInit()
		{
		}

		protected virtual void Localize()
		{

		}
		private void OnDrawGizmosSelected()
		{
			Rename();
		}
		[ContextMenu("Rename")]
		public void Rename()
		{
			string targetName = PageType.ToString() + "SP";
			if (gameObject.name != targetName)
				gameObject.name = targetName;
		}

		public void Hide()
		{
			RectTransform rt = GetComponent<RectTransform>();
			rt.DOAnchorPos(new Vector2(-rt.rect.width, 0), 0.4f).OnComplete(() =>
			{
				gameObject.SetActive(false);
				OnClose?.Invoke();
			});
		}

		public void Visible()
		{
			RectTransform rt = GetComponent<RectTransform>();
			rt.SetAsLastSibling();
			rt.anchoredPosition = new Vector2(-rt.rect.width, 0);
			gameObject.SetActive(true);
			rt.DOAnchorPos(Vector2.zero, 0.4f).OnComplete(()=>{
				OnVisibleComplete?.Invoke();
			});
		}
	}
}