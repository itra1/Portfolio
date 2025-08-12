using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetGame : ExEvent.EventBehaviour {

	public GameObject hintLetterIcon;

	private void OnEnable() {
		hintLetterIcon.gameObject.SetActive(false);

		hintLetterIcon.GetComponent<AnimationHelper>().OnEvent1 = () => {
			hintLetterIcon.gameObject.SetActive(false);
		};
	}
	
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterReady))]
	public void HintEnyLetterReady(ExEvent.GameEvents.OnHintAnyLetterReady hint) {
		hintLetterIcon.gameObject.SetActive(true);
		hintLetterIcon.GetComponent<Animation>().Play("show");
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterCompleted))]
	public void HintEnyLetterCompleted(ExEvent.GameEvents.OnHintAnyLetterCompleted hint) {
		//hintLetterIcon.gameObject.SetActive(false);
		hintLetterIcon.GetComponent<Animation>().Play("hide");
	}

}
