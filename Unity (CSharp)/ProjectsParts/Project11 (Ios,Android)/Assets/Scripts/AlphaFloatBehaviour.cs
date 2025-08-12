using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlphaFloatBehaviour : MonoBehaviour {

  public SpriteRenderer alphaSprite;
  public List<Sprite> aphabetSprites;
	private string _alpha;
	public Animation animComp;
	public GameObject graphic;

	public string alpha {
		get { return _alpha; }
		set {
			_alpha = value;
      alphaSprite.sprite = aphabetSprites.Find(x => x.name == _alpha.ToUpper());
		}
	}
	
	private void OnEnable() {
		graphic.SetActive(true);
    graphic.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-10, 10));

  }

	private void OnMouseEnter() {
		ExEvent.GameEvents.OnAlphaEnter.Call(transform.position,this);
	}

	private void OnMouseDown() {
		ExEvent.GameEvents.OnAlphaDown.Call(transform.position, this);
	}

	public void ShowCompleted() {

	}

	public void Show() {
		graphic.SetActive(true);
		animComp.Play("show");
	}

}
