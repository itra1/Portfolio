using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlphaFloatBehaviour : MonoBehaviour {

	public TextMeshProScale textMech;
	private string _alpha;
	public Animation animComp;
	public GameObject graphic;
	public RandomAnimation2 rndAnim;

	public string alpha {
		get { return _alpha; }
		set {
			_alpha = value;
			textMech.SetText(_alpha.ToUpper(), PlayerManager.Instance.company.actualCompany);
			textMech.transform.localEulerAngles = new Vector3(0,0,0);
		}
	}
	
	private void OnEnable() {
		graphic.SetActive(false);
		rndAnim.enabled = false;
	}

	private void OnMouseEnter() {
		ExEvent.GameEvents.OnAlphaEnter.Call(transform.position,this);
	}

	private void OnMouseDown() {
		ExEvent.GameEvents.OnAlphaDown.Call(transform.position, this);
	}

	public void ShowCompleted() {
		rndAnim.enabled = true;
	}

	public void Show() {
		graphic.SetActive(true);
		animComp.Play("show");
	}

}
