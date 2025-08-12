using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Weapon;

/// <summary>
/// Элемент
/// </summary>
public class BriefingWeapon : MonoBehaviour {
	
	public System.Action<BriefingWeapon, WeaponCategory> OnPointerDown;
	public System.Action<BriefingWeapon, WeaponCategory> OnPointerUp;
	public System.Action OnPointerDownNull;
	public System.Action OnPointerUpNull;
	public System.Action<BriefingWeapon, WeaponCategory> OnClick;

	public RectTransform rt;

	public WeaponCategory weaponCategory;

	public List<Image> listImage;
	public List<Text> listText;

	public GameObject content;

	public GameObject countObject;
	public Text countText;
  public Image icon;
	public Image iconNull;
	public Image backIconNull;
	public GameObject titleObject;
	public Text titleText;

	public RectTransform prt;

	public List<IconsData> iconData;

	public GameObject hunterIcon;
	public GameObject bearIcon;

	[System.Serializable]
	public struct IconsData {
		public WeaponType wt;
		public GameObject prefab;
	}

	public WeaponManager weaponManager { get; set; }
	
	public virtual void SetWeapon(WeaponManager weaponManager) {
		
		this.weaponManager = weaponManager;

		foreach (var iconDataElem in iconData)
			iconDataElem.prefab.SetActive(weaponManager != null && iconDataElem.wt == weaponManager.weaponType);
		
		if (weaponManager == null) {
			if (iconNull != null) iconNull.gameObject.SetActive(true);
			if (backIconNull != null) backIconNull.gameObject.SetActive(true);
			if (icon != null) icon.gameObject.SetActive(false);
			if (countObject != null) countObject.SetActive(false);
			if (titleObject != null) titleObject.SetActive(false);
			return;
		}
		if (iconNull != null) iconNull.gameObject.SetActive(false);
		if (backIconNull != null) backIconNull.gameObject.SetActive(false);
		if (icon != null) icon.gameObject.SetActive(true);

		if (countObject != null) countObject.SetActive(true);
		if (titleObject != null) titleObject.SetActive(true);

		if (icon != null) icon.sprite = weaponManager.IconActive;

		if (countText != null) {
		  WeaponManager wm = Game.User.UserWeapon.Instance.GetWeapon(weaponManager.weaponType);

      if(wm != null) {
        countText.text = wm.BulletCount.ToString();
      } else {

        var wep = GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)weaponManager.weaponType);

        countText.text = wep.startBullet.ToString();
      }

    }
		if (titleText != null) titleText.text = weaponManager.title;
	}
	
	private float scaleKoef = 1;
	private float showScale = 0;
	private float deltaShow = 0;

	float altPosition { get { return Mathf.Abs(prt.anchoredPosition.y + rt.anchoredPosition.y); } }

	public void CheckPos(bool isMini = true) {
		
		showScale = (isMini ? 80 : 150);

		if (altPosition < showScale) {
			scaleKoef = 1 - (altPosition / showScale);
			gameObject.transform.localScale = new Vector3(1 + (0.4f * scaleKoef), 1 + (0.4f * scaleKoef), 1);
		} else {
			gameObject.transform.localScale = Vector3.one;
		}
		
		if (isMini && altPosition > 80) {
			scaleKoef = ((115 - altPosition) / 35);
		} else if (!isMini && altPosition > 155) {
			scaleKoef = ((190 - altPosition) / 35);
		} else {
			scaleKoef = 1;
		}



		listImage.ForEach(x=>x.color = new Color(1,1,1, scaleKoef));
		listText.ForEach(x => x.color = new Color(1, 1, 1, scaleKoef));

		showScale = (isMini ? 80 : 150);

		//if (posStart > showScale) {
		//	deltaShow = posStart - showScale;
		//} else
		//	return;

		//scaleKoef = 1 - (deltaShow / 40);

		//gameObject.transform.localScale = new Vector3(1 + (0.4f * scaleKoef), 1 + (0.4f * scaleKoef), 1);

		//listImage.ForEach(x=>x.color = new Color(1,1,1, scaleKoef));
		//listText.ForEach(x => x.color = new Color(1, 1, 1, scaleKoef));

	}

	public void PointerDown() {
		isDrag = false;
		isPointerDown = true;
		UIController.ClickPlay();
		if (OnPointerDown != null) OnPointerDown(this, weaponCategory);
		if (OnPointerDownNull != null) OnPointerDownNull();
	}

	public void PointerUp() {
		if (!isPointerDown) return;
		isPointerDown = false;

		if (!isDrag && OnClick != null) OnClick(this, weaponCategory);

		if (OnPointerUp != null) OnPointerUp(this, weaponCategory);
		if (OnPointerUpNull != null) OnPointerUpNull();
	}

	bool isDrag;
	bool isPointerDown;
	
	public void Drag() {
		isDrag = true;
	}

}
