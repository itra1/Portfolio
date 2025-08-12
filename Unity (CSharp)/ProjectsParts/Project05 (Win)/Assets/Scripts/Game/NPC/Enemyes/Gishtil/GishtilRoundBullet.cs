using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level6.Gishtil
{
  public class GishtilRoundBullet : MonoBehaviourBase
  {
	 [SerializeField]
	 private ParticleSystem _particle;
	 [SerializeField]
	 private Collider _collider;
	 [SerializeField]
	 private Renderer _renderer;
	 [SerializeField]
	 [ColorUsage(true,true)]
	 private Color _tintColor;
	 [SerializeField]
	 private Transform _lightPrent;
	 [SerializeField]
	 private Light[] _light;

	 [SerializeField]
	 private float _lightIntencity = 6;

	 public ParticleSystem Particle { get => _particle; set => _particle = value; }

	 public void Cast(Vector3 position)
	 {
		transform.position = position;
		for(int i = 0; i < _light.Length; i++)
		{
		  _light[i].intensity = 0;
		}

		_lightPrent.localPosition = Vector3.zero;
		gameObject.SetActive(true);
		Activate();
	 }

	 public void Activate()
	 {
		Color startColor = _tintColor;
		startColor.a = 0;
		_renderer.material.SetColor("_TintColor", startColor);

		for (int i = 0; i < _light.Length; i++)
		{
		  Light l = _light[i];
		  l.DOIntensity(_lightIntencity, 1f).OnComplete(() => {
			 l.DOIntensity(0, 1f).SetDelay(2f);
		  });
		}
		_lightPrent.DOLocalMoveY(2, 4f);

		_renderer.material.DOColor(_tintColor, "_TintColor", 1f);

		DOVirtual.DelayedCall(1f, _particle.Play);
		DOVirtual.DelayedCall(1.1f, ()=> { _collider.enabled = true; });
		DOVirtual.DelayedCall(1.2f, () => { _collider.enabled = false; });
		DOVirtual.DelayedCall(1.2f, () => {
		
		});

		_renderer.material.DOColor(startColor, "_TintColor", 1f).SetDelay(3f).OnComplete(() => {
		  gameObject.SetActive(false);
		});

	 }

  }
}