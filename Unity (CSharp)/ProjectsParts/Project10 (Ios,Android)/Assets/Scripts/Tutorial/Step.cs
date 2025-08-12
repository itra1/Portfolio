using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial.Steps {

	public abstract class Step: ExEvent.EventBehaviour {

		public abstract int step { get; }

		public abstract string yaMetrica { get; }
		
		public abstract string gaDescr { get; }

		private void MetricLog() {

			YAppMetrica.Instance.ReportEvent(yaMetrica);

			GAnalytics.Instance.LogEvent("Туториал", "Выполнен", gaDescr, 1);

		}

		public virtual void StartStep() {
			
		}

		public virtual void Skip() {
			
		}

		public virtual void Update() {
			
		}

		public virtual void StepComplete() {
			MetricLog();

			TutorialController.Instance.StepComplete();

		}
		
		protected void InvokeMethod(Action method, float secondWait) {
			try {
				if (gameObject == null || !gameObject.activeInHierarchy) return;
				StartCoroutine(InvokeCoroutine(method, secondWait));
			}
			catch {
			}
		}

		private IEnumerator InvokeCoroutine(Action method, float secondWait) {
			yield return new WaitForSeconds(secondWait);
			method();
		}
	}

}