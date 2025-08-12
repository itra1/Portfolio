using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Weapon;

public class ScrollSelectWeapon : MonoBehaviour, IDragHandler {

	public RectTransform contentObject;
	public Action<BriefingWeapon, WeaponCategory> OnClose;

	private List<BriefingWeapon> weaponList = new List<BriefingWeapon>();

	public RectTransform rt;

	public BriefingWeapon weaponInst;
	public BriefingWeapon assistantInst;

	private Vector2 _laspPointerPosition;
	private float delta;

	float sizeY1 = 270;
	float sizeY2 = 450;

	private float minScroll = 0;
	private float maxScroll = 0;

	private float deltaElements = 90;

	private BriefingWeapon source;
	bool isMini;

	bool scrollLoop;

	bool isOpenScroll;
	
	public void SetListManagers(List<WeaponManager> readyManagers, BriefingWeapon source) {

		this.source = source;

		isOpenScroll = true;
		
		weaponList.ForEach(x => Destroy(x.gameObject));
		weaponList.Clear();

		for (int i = 0; i < readyManagers.Count; i++) {

			if (source.weaponCategory == WeaponCategory.asisstant) {
				GameObject inst = Instantiate(assistantInst.gameObject);
				inst.GetComponent<BriefingWeapon>().SetWeapon(readyManagers[i]);
				inst.transform.SetParent(contentObject);
				inst.transform.localScale = Vector3.one;
				inst.GetComponent<BriefingWeapon>().OnClick = WeaponClick;
				inst.GetComponent<BriefingWeapon>().OnPointerDownNull = PointDown;
				inst.GetComponent<BriefingWeapon>().OnPointerUpNull = PointUp;
				weaponList.Add(inst.GetComponent<BriefingWeapon>());
				inst.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, deltaElements * -i, 0);
			} else {
				GameObject inst = Instantiate(weaponInst.gameObject);

				inst.GetComponent<BriefingWeapon>().SetWeapon(readyManagers[i]);
				inst.transform.SetParent(contentObject);
				inst.transform.localScale = Vector3.one;
				inst.GetComponent<BriefingWeapon>().OnClick = WeaponClick;
				inst.GetComponent<BriefingWeapon>().OnPointerDownNull = PointDown;
				inst.GetComponent<BriefingWeapon>().OnPointerUpNull = PointUp;
				weaponList.Add(inst.GetComponent<BriefingWeapon>());
				inst.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, deltaElements * -i, 0);
			}
			//if (source.weaponManager == readyManagers[i]) {
			//	contentObject.anchoredPosition = new Vector2(contentObject.anchoredPosition.x, deltaElements * i);
			//}
		}

		contentObject.anchoredPosition = new Vector2(contentObject.anchoredPosition.x, 0);

		isMini = weaponList.Count <= 4;

		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (isMini ? sizeY1 : sizeY2));

		CheckPosition(true);

		weaponList.ForEach(x => x.CheckPos(isMini));
		weaponList.ForEach(x => x.gameObject.SetActive(true));
		scrollLoop = weaponList.Count > 3;

		if (!scrollLoop) {
			List<BriefingWeapon> sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
			maxScroll = sortList[sortList.Count - 1].GetComponent<RectTransform>().anchoredPosition.y;
			minScroll = sortList[0].GetComponent<RectTransform>().anchoredPosition.y;
		}

		if (this.source.weaponManager != null)
			MoveListToTarget();


	}

	void MoveListToTarget() {
		List<BriefingWeapon> sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
		float posY = 0;
		foreach (var elem in sortList)
			if (elem.weaponManager != null && elem.weaponManager.weaponType == this.source.weaponManager.weaponType)
				posY = elem.GetComponent<RectTransform>().anchoredPosition.y;
		float deltaY = contentObject.anchoredPosition.y + posY;

		int countDist = Math.Abs((int)(deltaY / deltaElements));
		for (int i = 0; i < countDist; i++) {
			contentObject.anchoredPosition += new Vector2(0, deltaElements * -Math.Sign(deltaY));
			if (scrollLoop)
				CheckPosition();
		}
		weaponList.ForEach(x => x.CheckPos(isMini));
	}

	void CheckPosition(bool isFirst = false, bool fullChange = true) {

		if (weaponList.Count <= 2) return;
		
		if (isFirst) {
			lastReOrder = contentObject.anchoredPosition.y;
			int afterMiddle = Mathf.CeilToInt((weaponList.Count - 1) / 2);
			
			for (int i = 0; i < afterMiddle; i++) {
				List<BriefingWeapon> sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
				sortList[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, sortList[sortList.Count - 1].GetComponent<RectTransform>().anchoredPosition.y + deltaElements, 0);
			}
		} else {

			float delta = lastReOrder - contentObject.anchoredPosition.y;
			if (fullChange) lastReOrder = contentObject.anchoredPosition.y;

			List<BriefingWeapon> sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
			
			if (delta > 0) {
				
				while (sortList[sortList.Count - 1].GetComponent<RectTransform>().anchoredPosition.y + contentObject.anchoredPosition.y < (90 * sortList.Count / 3)) {
					sortList[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, sortList[sortList.Count - 1].GetComponent<RectTransform>().anchoredPosition.y + deltaElements, 0);
					sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
				}
				
			} else if (delta < 0) {
				
				while (sortList[0].GetComponent<RectTransform>().anchoredPosition.y + contentObject.anchoredPosition.y > -(90 * sortList.Count / 3)) {
					sortList[sortList.Count - 1].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, sortList[0].GetComponent<RectTransform>().anchoredPosition.y - deltaElements, 0);
					sortList = weaponList.OrderBy(x => x.GetComponent<RectTransform>().anchoredPosition.y).ToList<BriefingWeapon>();
				}
			}
		}
	}

	private void OnEnable() {
		//PointDown();
		isPointer = false;
		_laspPointerPosition = Input.mousePosition;
	}

	private void OnDisable() {
		UseSelect();
		if (OnClose != null) OnClose(source, source.weaponCategory);
	}

	public void UseSelect() {
		int num = Mathf.RoundToInt(yPosition / deltaElements);

		if (Mathf.Abs(num) > weaponList.Count - 1)
			num = num % weaponList.Count;

		if (num < 0) num = (weaponList.Count) + num;

		this.source.GetComponent<BriefingWeapon>().SetWeapon(weaponList[num].weaponManager);
	}

	private float yPosition;

	private void Update() {

		if (isPointer || isFirstMove) {
			delta = Input.mousePosition.y - _laspPointerPosition.y;
			ScrollBlock(delta * 0.5f);
		} else {
			delta = Input.mouseScrollDelta.y * 20;
			if (delta != 0)
				ScrollBlock(delta);
		}


	}

	void WeaponClick(BriefingWeapon brif, WeaponCategory weaponCategory) {
		for (int i = 0; i < weaponList.Count; i++) {
			if (weaponList[i] == brif)
				yPosition = i * deltaElements;
		}
		gameObject.SetActive(false);
	}

	void ScrollBlock(float delta) {

		if (!isMove) isMove = delta != 0;
		_laspPointerPosition = Input.mousePosition;
		yPosition = contentObject.anchoredPosition.y + delta;

		if (!scrollLoop) {
			if (yPosition < minScroll) yPosition = minScroll;
			if (yPosition > maxScroll) yPosition = maxScroll;
		}
		
		contentObject.anchoredPosition = new Vector2(contentObject.anchoredPosition.x, yPosition);

		if (isMove)
			weaponList.ForEach(x => x.CheckPos(isMini));

		if (Math.Abs(lastReOrder - contentObject.anchoredPosition.y) > deltaElements / 3)
			CheckPosition();

	}

	float lastReOrder;

	private bool isPointer { get; set; }
	private bool isMove;
	private bool isFirstMove;

	public void SetFirstMove() {
		isFirstMove = true;
		isMove = false;
	}
	public void PointDown() {
		_laspPointerPosition = Input.mousePosition;
		isPointer = true;
		isMove = false;
		isFirstMove = false;
	}

	public void PointUp() {
		if (isFirstMove && isMove) {
			gameObject.SetActive(false);
			return;
		}
		isFirstMove = false;
		if (!isPointer) return;
		if (isPointer && !isMove) {
			gameObject.SetActive(false);
		}
		isPointer = false;
	}

	public void OnDrag(PointerEventData eventData) {
		Debug.Log(eventData.delta.y);
	}
}
