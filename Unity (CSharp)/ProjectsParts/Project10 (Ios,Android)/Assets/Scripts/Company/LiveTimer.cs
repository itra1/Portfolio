using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Company.Live {
  public class LiveTimer: MonoBehaviour {

    public GameObject block;
    public Text timerUi;
    public Text liveValueUi;

    private float value;

    private void OnEnable() {

      LiveCompany.OnChange += LiveChange;
      LiveChange(LiveCompany.Instance.value);
    }

    private void OnDisable() {

      LiveCompany.OnChange -= LiveChange;
      if (timerCor != null) {
        StopCoroutine(timerCor);
        timerCor = null;
      }
    }

    private void LiveChange(float value) {

      value = LiveCompany.Instance.value;
      liveValueUi.text = Mathf.Round(value).ToString();

      block.gameObject.SetActive(LiveCompany.Instance.maxValue > LiveCompany.Instance.value);

      if (LiveCompany.Instance.value < LiveCompany.Instance.maxValue) {
        timerCor = StartCoroutine(Timer());
      }

    }

    private Coroutine timerCor;

    private IEnumerator Timer() {

      while (LiveCompany.Instance.maxValue > LiveCompany.Instance.value) {

        timerUi.text = System.String.Format("{0:00}:{1:00}", LiveCompany.Instance.nextValue.Minutes, LiveCompany.Instance.nextValue.Seconds);
        yield return new WaitForSeconds(1);
      }

    }

  }
}