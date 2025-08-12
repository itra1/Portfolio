using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Диалоговое окно специальной продажи
/// </summary>
public class SpecialSalesDialog : PanelUi {

	public Action OnBye;

  private bool isClose;

  public AudioClip openClip;
  public Text timerText;
  public Text dialogButton;
  
  public GameObject goldLightObject;
  public Transform[] goldLightPosition;
  private float lastGoldLight;
  private float waitGoldLight = 1;

  protected override void OnEnable() {
		base.OnEnable();
    isClose = false;
    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffectDef, 0.5f);
    AudioManager.PlayEffect(openClip, AudioMixerTypes.shopEffect);
		
#if PLUGIN_VOXELBUSTERS
    //for(int i = 0; i < BillingController.product.Length; i++) {
    //  if(BillingController.productSDK[i].name == "Special")
    //    dialogButton.text = Lanuage.GetTranslate("ss_bye") + " " + BillingController.productSDK[i].price;
    //}
#endif

  }


	private TimeSpan timeDelta;
	private IEnumerator Timer() {

		while (true) {

			if (SpecialSales.Instance.nextShow < DateTime.Now) {
				timerText.text = "00:00:00";
			} else {
				timeDelta = SpecialSales.Instance.nextShow - DateTime.Now;
				timerText.text = String.Format("{0:00}:{1:00}:{2:00}", timeDelta.Hours, timeDelta.Minutes, timeDelta.Seconds);
			}

			yield return new WaitForSeconds(1);
		}
	}

	void Update() {
    DialogLightUpdate();
  }

  protected override void OnDisable() {
		base.OnDisable();
    isClose = false;
    AudioManager.SetSoundMixer(AudioSnapshotTypes.mapEffect0, 0.5f);
		if (OnClose != null) OnClose();
  }


  void ChangeTimer(string textTimer) {
    timerText.text = textTimer;
  }

  /// <summary>
  /// Закрываем диалог
  /// </summary>
  public void Close() {
    isClose = true;
    UiController.ClickButtonAudio();
    AudioManager.PlayEffect(openClip, AudioMixerTypes.shopEffect);
    gameObject.GetComponent<Animator>().SetTrigger("close");
  }

	public void ButtonBye() {
		if (OnBye != null) OnBye();
	}

  public void ClosePanel() {
    if(!isClose) return;
    isClose = false;
    gameObject.SetActive(false);
  }

  void DialogLightUpdate() {
    if(lastGoldLight + waitGoldLight < Time.time) {
      lastGoldLight = Time.time;
      waitGoldLight = UnityEngine.Random.Range(1f, 4f);
      goldLightObject.transform.position = goldLightPosition[UnityEngine.Random.Range(0, goldLightPosition.Length)].position;
      goldLightObject.GetComponent<Animator>().SetTrigger("play");
    }
  }

	public override void BackButton() {
		Close();
	}
}
