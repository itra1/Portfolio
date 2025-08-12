using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR

using UnityEditor;


[CustomEditor(typeof(WeaponBehaviour))]
[CanEditMultipleObjects]
public class WeaponBehaviourEditor: Editor {

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    if (GUILayout.Button("Rename"))
    {
      for (int i = 0; i < targets.Length; i++) {

        WeaponBehaviour wb = (WeaponBehaviour)targets[i];
        string name = AssetDatabase.GetAssetPath(wb.gameObject);
        AssetDatabase.RenameAsset(name, WeaponManager.GetName(wb.playerType, wb.type, wb.hand) + ".prefab");
      }

    }

  }

}

#endif

public class WeaponBehaviour : Singleton<WeaponBehaviour> {

	public Action OnStartAttack;
	public Action OnEndAttack;

	public WeaponType type;
	public PlayerType playerType;
	public Hand hand;

	public bool freezRotationX;
	public bool freezRotationY;
	public bool freezRotationZ;
	public bool freezRotationW;

	private PlayerBehaviour _parentPlayer;

	private List<MeshRenderer> _meshLiast = new List<MeshRenderer>();

	//[HideInInspector]
	public Transform parent;

	private void OnEnable() {

		float scl = MapManager.Instance.map.playerSize;
		transform.localScale = new Vector3(scl*0.2f, scl * 0.2f, scl * 0.2f);
	}
	
	private void OnDisable() {
		_parentPlayer.OnStartAttack -= StartAttack;
		_parentPlayer.OnEndAttack -= EndAttack;
		_parentPlayer.OnChangeVisible -= OnChangeVisible;
	}
	
	public void InitData(PlayerBehaviour parent) {
		_parentPlayer = parent;
		_parentPlayer.OnStartAttack += StartAttack;
		_parentPlayer.OnEndAttack += EndAttack;
		_parentPlayer.OnChangeVisible += OnChangeVisible;
		OnChangeVisible(_parentPlayer.isVisible);
	}

	public void OnChangeVisible(bool isVisible) {

		if (_meshLiast.Count == 0) {
			var _meshLiastTmp = GetComponentsInChildren<MeshRenderer>();
			_meshLiast = _meshLiastTmp.ToList();
		}

		for (int i = 0; i < _meshLiast.Count; i++) {
			_meshLiast[i].enabled = isVisible;
		}

	}

	void StartAttack() {
		if (OnStartAttack != null) OnStartAttack();
	}

	void EndAttack() {
		if (OnEndAttack != null) OnEndAttack();
	}

	private void LateUpdate() {

		if (parent == null) return;

		if (!parent.gameObject.activeInHierarchy) {
			_parentPlayer = null;
			return;
		}

		transform.position = parent.position;
		
		transform.eulerAngles = new Vector3(freezRotationX ? transform.eulerAngles.x : parent.eulerAngles.x,
																				freezRotationY ? transform.eulerAngles.y : parent.eulerAngles.y,
																				freezRotationZ ? transform.eulerAngles.z : parent.eulerAngles.z);
	}
  
}

public enum Hand {
	none = 0,
	right = 1,
	left = 2,
	all = 3
}