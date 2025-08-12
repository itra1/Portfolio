using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet7 : MonoBehaviourBase
  {
	 [SerializeField] private LaserLineV3D _laser;
	 [SerializeField] private Renderer _renderer;
	 [ColorUsage(true, true)]
	 [SerializeField] private Color _tintColor;
	 [SerializeField] private Transform _lightPrent;
	 [SerializeField] private Light[] _light;
	 [SerializeField] private Collider _collider;

	 [SerializeField]
	 private float _lightIntencity = 6;

	 private void Start()
	 {
		_renderer.material = Instantiate(_renderer.material);
	 }

	 private void OnEnable()
	 {
		_laser.maxLength = 0;
		for (int i = 0; i < _light.Length; i++)
		{
		  _light[i].intensity = 0;
		}

		Activate();
	 }

	 public void Activate()
	 {
		Color startColor = _tintColor;
		startColor.a = 0;
		_renderer.material.SetColor("_TintColor", startColor);

		//_renderer.material.DOColor(_tintColor, "_TintColor", 0.5f);

		DOTween.To(() => _renderer.material.GetColor("_TintColor"),
		 (x) => _renderer.material.SetColor("_TintColor", x),
		  _tintColor, 0.5f);

		_lightPrent.localPosition = new Vector3(0, 2, 0);

		DOVirtual.DelayedCall(0.7f, () =>
		{

		  for (int i = 0; i < _light.Length; i++)
		  {
			 Light l = _light[i];
			 l.DOIntensity(_lightIntencity, 1f).OnComplete(() => {
				l.DOIntensity(0, 1f).SetDelay(1.5f);
			 });
		  }

		  DOTween.To(() => _laser.maxLength, (x) => _laser.maxLength = x, 10, 0.5f).OnComplete(()=> {

			 DOTween.To(() => _laser.maxLength, (x) => _laser.maxLength = x, 0, 0.5f).SetDelay(2f);

		  });
		  DOVirtual.DelayedCall(0.2f, () => {
			 _collider.enabled = true;

			 DOVirtual.DelayedCall(2.5f, () => { _collider.enabled = false; });

		  });

		  _renderer.material.DOColor(startColor, "_TintColor", 0.5f).SetDelay(3f).OnComplete(() => {
			 gameObject.SetActive(false);
		  });

		 // DOTween.To(() => _renderer.material.GetColor("_TintColor"),
			//(x) => _renderer.material.SetColor("_TintColor", x),
			//  startColor, 0.5f).SetDelay(4f).OnComplete(() => {
			//	 gameObject.SetActive(false);
			//  });

		});
	 }

  }
}