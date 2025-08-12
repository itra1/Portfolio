using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseBehaviour : MonoBehaviour {

	public Transform parent;

	public Animator animComponent;
	
	public SkinnedMeshRenderer[] allRenderer;
	public MeshRenderer[] allMeshRenderer;

	public bool isDead = false;
	private bool moveDown = false;

	private void OnEnable() {
		
		isDead = false;
		moveDown = false;
		SetMove(false);

	  float scl = MapManager.Instance.map.playerSize;
	  transform.localScale = new Vector3(scl * 0.2f, scl * 0.2f, scl * 0.2f);

  }

  public void SetMove(bool isMove) {
		if (isDead) return;
		animComponent.SetBool("move", isMove);
	}

	public void Dead() {
		isDead = true;
		animComponent.SetTrigger("dead");
		Invoke("SetModeDown", 10);
	}
	
	private void Awake() {
		GetMeshes();
	}
	
	private void Update() {
		if (moveDown) {
			transform.position += Vector3.down*1*Time.deltaTime;
			if (transform.position.y <= -15) {
				gameObject.SetActive(false);
			}
		}
	}

	private void SetModeDown() {
		moveDown = true;
	}
	
	private void GetMeshes() {
		allRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
		allMeshRenderer = GetComponentsInChildren<MeshRenderer>();
	}

	public void SetVisibleFog(bool isVisible) {

		if (allRenderer.Length == 0)
			GetMeshes();

		for (int i = 0; i < allRenderer.Length; i++)
			allRenderer[i].enabled = isVisible;
		for (int i = 0; i < allMeshRenderer.Length; i++)
			allMeshRenderer[i].enabled = isVisible;

	}
	
}
