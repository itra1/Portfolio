using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageShopDialog : MonoBehaviour {

	public event Action OnSelect;
	public Transform page;
	public IapType type;
	public bool _isActive;
	public ProductShopDialog productInst;

	public LocalizationUiText helper;

	public string description;

	public Sprite icon;

	public Transform _pageContent;

	public Sprite firstPack;
	public Sprite allLocation;

	private void Start() {
		var elements = BillingManager.Instance.GetBillingProduct(type);

		if (type == IapType.pack) {
			elements.Add(BillingManager.Instance.GetBillingProduct(IapType.unlimitedPack)[0]);
			elements.Add(BillingManager.Instance.GetBillingProduct(IapType.locationAll)[0]);
		}

		for (int i = 0; i < elements.Count; i++) {
			GameObject inst = Instantiate(productInst.gameObject);
			inst.gameObject.SetActive(true);
			inst.transform.SetParent(_pageContent);
			inst.transform.localScale = Vector3.one;
			inst.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-30 -i*220);
			if (type == IapType.pack && i == 0) {
				inst.GetComponent<ProductShopDialog>().SetData(i, elements[i], firstPack);
			}
			else if(type == IapType.pack && i == 2) {
				inst.GetComponent<ProductShopDialog>().SetData(i, elements[i], allLocation);
			}else
				inst.GetComponent<ProductShopDialog>().SetData(i, elements[i], icon);
			OnSelect += inst.GetComponent<ProductShopDialog>().OnSelectPage;
		}
		
	}
	
	public void SetActive(bool isActive) {
		this._isActive = isActive;

		_pageContent.gameObject.SetActive(this._isActive);


		if (this._isActive) {
				helper.SetCode(description);
				if (OnSelect != null) OnSelect();
		}

	}
}
