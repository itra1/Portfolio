using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConchUi : MonoBehaviour {
	
	public List<GameObject> graphics;
	public Animation animMain;

	public ParticleSystem partic;

	private int num;

	public GameObject pointer;

	private void Awake() {
		ChangeValue();
	}

	private System.Action OnDeactive;
	public void SetPointer(bool isActive, System.Action OnDeactive = null) {
		if(pointer != null)
			pointer.SetActive(isActive);
		if (isActive) {
			this.OnDeactive = OnDeactive;
		}
		else {
			if (this.OnDeactive != null)
				this.OnDeactive();
			this.OnDeactive = null;
		}
	}

	private void OnEnable() {
		ConchManager.OnSetValue += ChangeValue;
		ConchManager.OnUse += UseCounch;
		num = ConchManager.Instance.actualValue;
		ConfirmData();
	}

	private void OnDisable() {
		ConchManager.OnSetValue -= ChangeValue;
		ConchManager.OnUse -= UseCounch;
	}

	public void ChangeValue() {

		if (GameManager.gamePhase == GamePhase.game) return;
		ConfirmData();
	}

	private void UseCounch() {
		ConfirmData();
	}

	void ConfirmData() {

		for (int i = 0; i < graphics.Count; i++)
			graphics[i].SetActive(i == ConchManager.Instance.actualValue);

		if (ConchManager.Instance.actualValue >= 10)
			graphics[graphics.Count - 1].SetActive(true);
	}

	public void AddNew() {
		/*num++;
		for (int i = 0; i < graphics.Count; i++)
			graphics[i].SetActive(i == num);
			*/
		ConfirmData();
		if (animMain != null)
			animMain.Play("addConch");

		partic.Play();

		if (ConchManager.Instance.actualValue > 10)
			graphics[graphics.Count - 1].SetActive(true);

	}

	// Клик
	public void Click() {
		SetPointer(false);
		//AudioManager.Instance.library.PlayClickAudio();
		ConchManager.Instance.ClickIcon();
	}


}
