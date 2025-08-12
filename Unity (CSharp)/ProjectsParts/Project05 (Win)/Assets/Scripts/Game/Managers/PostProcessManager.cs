using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using JetBrains.Annotations;
using DG.Tweening;

namespace it.Game.Managers
{
  public class PostProcessManager : Singleton<PostProcessManager>
  {
	 [SerializeField]
	 private PostProcessVolume _basePostProcess;

	 public PostProcessVolume BasePostProcess { get => _basePostProcess; set => _basePostProcess = value; }

	 [SerializeField]
	 private PostProcessVolume _playerForm2Process;
	 public PostProcessVolume PlayerForm2Process { get => _playerForm2Process; set => _playerForm2Process = value; }

	 [SerializeField]
	 private PostProcessVolume _fullLight;
	 public PostProcessVolume FullLight { get => _fullLight; set => _fullLight = value; }

	 [SerializeField]
	 private PostProcessVolume _special2;
	 public PostProcessVolume Special2 { get => _special2; set => _special2 = value; }


	 private void Start()
	 {
		_special2.weight = 0;
		_special2.enabled = false;
		_playerForm2Process.weight = 0;
		FullLight.weight = 0;
	 }

	 public void PlayFullLight(UnityEngine.Events.UnityAction OnMiddle,
		UnityEngine.Events.UnityAction OnComplete,
		float duration = 0.5f,
		float middlePause = 0)
	 {
		DOTween.To(() => _fullLight.weight, (x) => _fullLight.weight = x, 1, duration / 2).OnComplete(() =>
		{
		  OnMiddle?.Invoke();
		  DOTween.To(() => _fullLight.weight, (x) => _fullLight.weight = x, 0, duration / 2).OnComplete(() =>
		  {
			 OnComplete?.Invoke();
		  }).SetDelay(middlePause);
		});
	 }

	 public void ActiveSpecial2(bool isActive, UnityEngine.Events.UnityAction onComplete)
	 {
		_special2.enabled = true;
		float targetValue = isActive ? 1 : 0;

		DOTween.To(() => _special2.weight, (x) => _special2.weight = x, targetValue, 1).OnComplete(() =>
		{
		  onComplete?.Invoke();
		  if(!isActive)
			 _special2.enabled = false;
		});
	 }

  }
}