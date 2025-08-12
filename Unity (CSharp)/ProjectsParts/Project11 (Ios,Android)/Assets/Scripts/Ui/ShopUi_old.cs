using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUi_old : UiPanel {

	//public GameObject locerObject;
	
	public void CloseButton() {
		//AudioManager.Instance.library.PlayClickAudio();
		if (isClosing) return;
		isClosing = true;
		Hide(() => {
			isClosing = false;
			gameObject.SetActive(false);
		});
	}
	
	public List<CategoryShopDialog> categoryList;

	public IapType _activeType;
	public Animation animComp;

	protected override void OnEnable() {
		base.OnEnable();
		BillingManager.OnCompleted += OnByeProduct;
		//locerObject.gameObject.SetActive(false);
	}

	protected override void OnDisable() {
		base.OnDisable();
		BillingManager.OnCompleted -= OnByeProduct;
	}

	void OnByeProduct() {
		//locerObject.gameObject.SetActive(false);
	}

	public void Start() {
		categoryList.ForEach(x=>x.OnClick = Click);
	}

	public void Click(IapType product) {
		if (_activeType == product) return;
		SetType(product);
	}

	public void SetType(IapType type) {

		_activeType = type;

		categoryList.ForEach(x=>x.SetActive(x.type == _activeType));
		
	}

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		AudioManager.Instance.library.PlayWindowOpenAudio();
		animComp.Play("show");
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		AudioManager.Instance.library.PlayWindowCloseAudio();
		animComp.Play("hide");
	}

	public void ByeProduct(BillingProductAbstract bill, Action callback) {

		//locerObject.gameObject.SetActive(true);
		BillingManager.Instance.ByeProduct(bill, callback);
	}

	public override void ManagerClose() {
		CloseButton();
	}
}
