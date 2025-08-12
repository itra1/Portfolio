using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RateDialog : PanelUi {

	public Text description;
	public Color activeColor;
	public Color disActiveColor;

	public Image[] stars;

	protected override void OnEnable() {
		base.OnEnable();
		description.text = LanguageManager.GetTranslate("dlg_RateDesc");
		for (int i = 0; i < stars.Length; i++)
			stars[i].color = disActiveColor;
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	public void CloseThis() {
		UiController.ClickButtonAudio();
		GetComponent<Animator>().SetTrigger("close");
		StartCoroutine(waitAndDisable());
	}

	IEnumerator waitAndDisable() {
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}

	public void ButtonYes(int num) {

		UiController.ClickButtonAudio();

		for (int i = 0; i < stars.Length; i++) {
			if (i <= num - 1)
				stars[i].color = activeColor;
		}

		PlayerPrefs.SetInt("rate", num);

		// if (num == 5)
#if PLUGIN_VOXELBUSTERS
		if (num >= 4/* && VoxelBusters.NativePlugins.NPSettings.Utility.RateMyApp.IsEnabled*/) {
			NPBinding.Utility.RateMyApp.AskForReviewNow();
			//NPBinding.Utility.OpenStoreLink(VoxelBusters.NativePlugins.NPSettings.Application.StoreIdentifier);
		}
#endif
		CloseThis();
	}

	public override void BackButton() {
		CloseThis();
	}
}
