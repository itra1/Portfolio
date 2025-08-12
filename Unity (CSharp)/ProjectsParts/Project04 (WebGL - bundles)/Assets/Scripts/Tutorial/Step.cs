using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

	1 - Первоначальная инициализация, создание игрока и и врага


	*/

namespace Tutorial.Steps {

	public abstract class Step : ExEvent.EventBehaviour {
		
		public abstract int step { get; }

		public virtual void StartStep() {

		}

		public virtual void Init() {
			
		}

		public virtual void StepComplete() {

			Tutorial.Instance.StepComplete();

		}

		protected void InvokeMethod(System.Action method, float secondWait) {
			try {
				if (gameObject == null || !gameObject.activeInHierarchy) return;
				StartCoroutine(InvokeCoroutine(method, secondWait));
			} catch {
			}
		}

		private IEnumerator InvokeCoroutine(System.Action method, float secondWait) {
			yield return new WaitForSeconds(secondWait);
			method();
		}

	}
}