using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace it.UI.Profiles
{

	public abstract class BaseUserProfilePage : MonoBehaviour
	{

		protected virtual void OnEnable(){

		}

		protected virtual void OnDisable()
		{

		}

		public void Hide(){

			RectTransform rt = GetComponent<RectTransform>();
			rt.DOAnchorPos(new Vector2(-rt.rect.width, 0), 0.4f).OnComplete(()=> {
				gameObject.SetActive(false);
			});
		}

		public void Visible(){

			RectTransform rt = GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(-rt.rect.width, 0);
			gameObject.SetActive(true);
			rt.DOAnchorPos(Vector2.zero, 0.4f);
		}

	}
}