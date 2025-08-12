using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class TutorialTarget1 : MonoBehaviour {

	public string functionNameForInvoke;
	public float functionInvoceTime;
	[SerializeField] Coin coin;
	[SerializeField] BarrierController barrier;

	[SerializeField] bool forceInvoce;              // Флаг моментального подтверждения

	bool _isActive = true;

	enum TutorObjectPhase {
		none, confirm, failed
	}
	TutorObjectPhase phaseElement;


	void Start() {
		Tutorial.TutorialController.OnEndTutorial += OnEndTutorial;
		phaseElement = TutorObjectPhase.none;

		if (coin != null)
			coin.OnGetPlayer += CoinsGet;
		if (barrier != null)
			barrier.OnBarrierDestroy += OnBarrierDestroy;
	}

	private Action OnComplete;
	private Action OnFailed;

	public void SetCallback(Action OnComplete, Action OnFailed) {
		this.OnComplete = OnComplete;
		this.OnFailed = OnFailed;
	}

	void OnDestroy() {
		Tutorial.TutorialController.OnEndTutorial -= OnEndTutorial;
		if (phaseElement == TutorObjectPhase.confirm) {
			if (OnComplete != null) OnComplete();
		}
		else {
			if (OnFailed != null) OnFailed();
		}
	}

	void Update() {
		if (transform.position.x <= CameraController.displayDiff.leftDif(2)) {
			Destroy(gameObject);
		}
	}

	void OnBarrierDestroy() {
		Debug.Log("event destroy");
		phaseElement = TutorObjectPhase.failed;
	}

	void CoinsGet() {
		if (forceInvoce) {
			NextStep();
		} else {
			if (phaseElement == TutorObjectPhase.none)
				phaseElement = TutorObjectPhase.confirm;
		}
	}


	void OnEndTutorial() {
		if (gameObject == null)
			return;
		GetComponent<Animator>().SetTrigger("close");
		Destroy(gameObject, 4);
	}

	void NextStep() {

		if (!_isActive || phaseElement != TutorObjectPhase.none)
			return;
		phaseElement = TutorObjectPhase.confirm;

		_isActive = false;
		GetComponent<Animator>().SetTrigger("close");
		Destroy(gameObject, 4);
		//GameObject.Find("Tutorial").GetComponent<TutorialController>().InvoceMerhod(functionNameForInvoke , functionInvoceTime);
		if (OnComplete != null) OnComplete();
		//Tutorial.TutorialController.Instance.InvoceMerhod(functionNameForInvoke, functionInvoceTime);
	}

}
