using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace it.UI.FirstLogo
{
  public class FirstLogo : UIDialog
  {
	 [SerializeField]
	 private Image[] _logos;

	 private Image _target;

	 public UnityEngine.Events.UnityAction onLogoShowComplete;

	 private PhaseType _phase;
	 private int _index = 0;

	 private Tweener _tweener;

	 private enum PhaseType
	 {
		wait,
		visible,
		active,
		hide
	 }

	 protected override void OnEnable()
	 {
		base.OnEnable();

		for(int i = 0; i < _logos.Length; i++)
		{
		  _logos[i].gameObject.SetActive(false);
		  _logos[i].color = new Color(1, 1, 1, 0);
		}
		_index = -1;
		ShowNext();
	 }

	 private void ShowNext()
	 {
		_index++;

		if (_index >= _logos.Length)
		{
		  onLogoShowComplete?.Invoke();
		  return;
		}

		_target = _logos[_index];

		_phase = PhaseType.wait;
		_tweener = _target.DOColor(Color.white, 2).SetDelay(1)
		  .OnStart(() =>
		  {
			 _phase = PhaseType.visible;
			 _target.gameObject.SetActive(true);
		  })		  
		  .OnComplete(() =>
		  {
			 _phase = PhaseType.active;
			 _tweener = _target.DOColor(new Color(1, 1, 1, 0), 2).SetDelay(5).OnStart(() =>
			 {
				_phase = PhaseType.hide;
			 }).OnComplete(()=> {

				ShowNext();
			 });
		  });

	 }
	 public void Next()
	 {
		if (_phase != PhaseType.active && _phase != PhaseType.visible)
		  return;

		_tweener.Kill();

		_phase = PhaseType.active;
		_tweener = _target.DOColor(new Color(1, 1, 1, 0), 2).OnStart(() =>
		{
		  _phase = PhaseType.hide;
		}).OnComplete(() => {

		  ShowNext();
		});

	 }

	 private void Update()
	 {
		if (Input.anyKey)
		{
		  Next();
		}
	 }

	 public override void Escape()
	 {
		base.Escape();
		Next();
	 }


	 //private void ShowLogos()
	 //{
	 //DOTween.To(() => _logo1.color, (x) => _logo1.color = x, Color.white, 4).SetDelay(2)
	 //  .OnStart(() =>
	 //  {
	 //	 _logo1.gameObject.SetActive(true);
	 //  })
	 //  .OnComplete(() =>
	 //  {
	 //  DOTween.To(() => _logo1.color, (x) => _logo1.color = x, new Color(1, 1, 1, 0), 2).SetDelay(5);
	 //  });

	 //DOTween.To(() => _logo2.color, (x) => _logo2.color = x, Color.white, 4).SetDelay(15)
	 //  .OnStart(() =>
	 //  {
	 //	 _logo2.gameObject.SetActive(true);
	 //  })
	 //  .OnComplete(() =>
	 //  {
	 //  DOTween.To(() => _logo2.color, (x) => _logo2.color = x, new Color(1, 1, 1, 0), 2).SetDelay(5).OnComplete(() =>
	 //  {
	 //	 onLogoShowComplete?.Invoke();
	 //  });

	 //});
	 //}


  }
}