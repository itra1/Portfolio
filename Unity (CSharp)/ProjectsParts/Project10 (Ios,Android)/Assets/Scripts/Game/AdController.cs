using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using Spine.Unity;

public class AdController : MonoBehaviour {

	public RunnerController runner;
	public GameObject birdObj;
	public GameObject stoneObj;
	public GameObject dialogObj;
	public GameObject textObj;

	public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
	string currentAnimation;                            // Текущая анимация

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string idleAnim = "";                        // Анимация в стандартном положении

	AdContent adContent;

	bool thisShow;

	void Start() {
		SetAnimation(idleAnim, true);
		thisShow = true;
		dialogObj.SetActive(false);
		GetBanner();
	}

	public void GetBanner() {
		if (!IsInvoking("GetBannerNow")) Invoke("GetBannerNow", 1f);
	}

	public void GetBannerNow() {
		Apikbs.Instance.GetAd(GetContent);
	}

	void GetContent(AdContent val) {

		if (val.url == null || val.url == "") return;

		adContent = val;
		if (adContent.title != "" & thisShow) {

			textObj.GetComponent<TextMesh>().text = val.title; //.Replace(' ', '\n');
			StartCoroutine(ShowBanner());
		}
	}

	IEnumerator ShowBanner() {
		bool ready = true;

		while (ready) {
			yield return new WaitForEndOfFrame();
			if (transform.position.y < CameraController.displayDiff.transform.position.y + CameraController.displayDiff.down) {
				transform.position += Vector3.up * Time.deltaTime;
			} else {
				//Debug.Log(transform.position.y + " - " + CameraController.displayDiff.transform.position.y + CameraController.displayDiff.down + 1);
				ready = false;
				changeShowAd(true);
			}

			// Аварийно завершаем
			if (!thisShow) ready = false;
		}
	}

	public void Click() {

		//if(!UiController.checkStart()) return;
		if (runner.runnerPhase != RunnerPhase.start) return;

		ShowAd(adContent.url);
	}

	public void ShowAd(string url) {

		if (url == null || url == "") return;

		Regex regex = new Regex(@"(?<=id)\d{4,}");

		Match match = regex.Match(url);

		if (match != null && match.Success) storeManager.showStoreWithProduct(match.Groups[0].Value);


		//Application.OpenURL(url);

	}

	public void pause(bool flag) {
		if (flag) {
			skeletonAnimation.timeScale = 0;
		} else {
			skeletonAnimation.timeScale = 1;
		}
	}

	public void HideAd() {
		thisShow = false;

		changeShowAd(false);
		StartCoroutine(HideBanner());
	}

	IEnumerator HideBanner() {
		bool ready = true;

		while (ready) {
			yield return new WaitForEndOfFrame();
			if (transform.position.y < CameraController.displayDiff.transform.position.y + CameraController.displayDiff.down + 1) {
				transform.position += Vector3.down * Time.deltaTime;
			} else {
				ready = false;
			}
		}
	}

	void changeShowAd(bool flag) {
		dialogObj.SetActive(flag);
	}


	#region Animation
	/* ***************************
	 * Применение анимации
	 * ***************************/
	public void SetAnimation(string anim, bool loop) {
		if (currentAnimation != anim) {
			skeletonAnimation.state.SetAnimation(0, anim, loop);
			currentAnimation = anim;
		}
	}


	/* ***************************
	 * Резет анимации
	 * ***************************/
	public void ResetAnimation() {
		skeletonAnimation.Initialize(true);
		currentAnimation = null;
	}


	/* ***************************
	 * Добавленная анимация
	 * ***************************/
	public void AddAnimation(int index, string animName, bool loop, float delay) {
		skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
	}

	/* ***************************
	 * Установка скорости
	 * ***************************/
	public void SpeedAnimation(float speed) {
		skeletonAnimation.timeScale = speed;
	}
	#endregion


}
