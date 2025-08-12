using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using I2.Loc;
using it.Game.Managers;

namespace it.UI.Cartoon
{
  public class CartoonDialog : UIDialog
  {
	 private it.Cartoons.Cartoon _cartoon;
	 GameObject cartoonInstance;
	 public void Play(it.Cartoons.Cartoon cartoon)
	 {
		cartoonInstance = Instantiate(cartoon.gameObject, transform);
		cartoonInstance.SetActive(false);
		_cartoon = cartoonInstance.GetComponent<it.Cartoons.Cartoon>();
		_cartoon.onShowComplete = ShowComplete;
		_cartoon.onComplete = cartoon.onComplete;

		if (cartoon.fideIn)
		{

		  UiManager.Instance.FillAndRepeatColor(0.5f, null, () => {
			 cartoonInstance.SetActive(true);
		  }, () => {
			 cartoonInstance.GetComponent<Animator>().SetTrigger("Play");
		  }, 0);
		}
		else
		{
		  cartoonInstance.SetActive(true);
		  cartoonInstance.GetComponent<Animator>().SetTrigger("Play");
		}


	 }

	 private void ShowComplete()
	 {
		if (_cartoon.fideOut)
		{

		  UiManager.Instance.FillAndRepeatColor(0.5f, null, () => {
			 Destroy(cartoonInstance);
		  }, () => {
			 _cartoon.onComplete?.Invoke();
		  }, 0);
		}
		else
		{
		  Destroy(cartoonInstance);
		  _cartoon.onComplete?.Invoke();
		}

	 }

  }
}