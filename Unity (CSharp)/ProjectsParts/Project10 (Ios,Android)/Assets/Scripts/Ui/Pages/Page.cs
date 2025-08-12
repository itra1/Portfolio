using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shop.View.Pages {
	
	public enum PageType {
		upgrades,
		powerUps,
		clothes,
		mounts,
		golds
	}

	public class Page : MonoBehaviour {

		public PageType pageType;

		public RectTransform parentItems;

	}
}