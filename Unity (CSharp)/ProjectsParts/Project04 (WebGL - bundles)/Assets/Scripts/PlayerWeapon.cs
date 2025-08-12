using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerWeapon))]
public class PlayerWeaponEditor : Editor {
	private WeaponType wl;
	private WeaponType wr;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Left");
		wl = (WeaponType)EditorGUILayout.EnumPopup(wl);
		if (GUILayout.Button("Set")) {
			((PlayerWeapon)target).SetWeapon(wl, Hand.left);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Right");
		wr = (WeaponType)EditorGUILayout.EnumPopup(wr);
		if (GUILayout.Button("Set")) {
			((PlayerWeapon)target).SetWeapon(wr, Hand.right);
		}
		GUILayout.EndHorizontal();
	}
}

#endif

public class PlayerWeapon : MonoBehaviour {

	public PlayerBehaviour pb;
	public IAnimationPlayer _pa;

	public IAnimationPlayer pa {
		get {

			if (_pa == null)
				_pa = GetComponent<IAnimationPlayer>();
			return _pa;

		}
	}

	public Transform leftWeaponParent;
	public Transform rightWeaponParent;
	public Transform spearWeaponParent;

	[HideInInspector]
	public WeaponBehaviour activeLeftWeapon;
	[HideInInspector]
	public WeaponBehaviour activeRightWeapon;

	public Hand activeRightHand;
	public Hand activeLeftHand;

	public void SetWeapon(WeaponType wt, Hand hand) {
		
		if (activeLeftWeapon != null && hand == Hand.left && activeLeftWeapon.type == wt) return;
		
		if (activeRightWeapon != null && hand == Hand.right && activeRightWeapon.type == wt) return;

		switch (hand) {
				case Hand.left:

				if (activeLeftWeapon != null)
					activeLeftWeapon.gameObject.SetActive(false);

				activeLeftWeapon = null;
				activeLeftHand = Hand.none;
				break;
				case Hand.right:

				if (activeRightWeapon != null)
					activeRightWeapon.gameObject.SetActive(false);

				activeRightWeapon = null;
				activeRightHand = Hand.none;
				break;
		}
		

		if (wt != WeaponType.none) {

			WeaponBehaviour useWb = WeaponManager.Instance.GetWeapon(pb.type, wt, hand);

      if(useWb == null)
        return;

			useWb.gameObject.SetActive(true);
			if (hand == Hand.left) {
				activeLeftWeapon = useWb;
				useWb.parent = leftWeaponParent;
				activeLeftHand = Hand.left;
			} else {
				activeRightWeapon = useWb;
				activeRightHand = Hand.none;
				if (wt == WeaponType.pike) {
					hand = Hand.all;
					activeRightHand = Hand.all;
					useWb.parent = spearWeaponParent;
				}
				else if (wt == WeaponType.bow) {
					useWb.parent = rightWeaponParent;
					hand = Hand.all;
					activeRightHand = Hand.all;
					useWb.GetComponent<BowWeapon>().parentString = leftWeaponParent;
				}
				else
					useWb.parent = rightWeaponParent;
			}

			useWb.InitData(pb);
		}

		pa.SetWeapon(wt, hand);
		if(pb.isMain)
			ExEvent.BattleEvents.OnPlayerWeaponChange.Call();
	}

	public void DeactiveWeapon(bool isLeft = false) {

		if (isLeft) {
			activeLeftWeapon.gameObject.SetActive(false);
			activeLeftWeapon = null;
		} else {
			activeRightWeapon.gameObject.SetActive(false);
			activeRightWeapon = null;

		}

	}



}
