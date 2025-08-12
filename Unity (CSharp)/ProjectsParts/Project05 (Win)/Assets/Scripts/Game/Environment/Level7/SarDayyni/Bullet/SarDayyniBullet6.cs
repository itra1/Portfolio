using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.VFX;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet6 : MonoBehaviourBase
  {

	 [SerializeField] private LaserLineV3D _laser;
	 [SerializeField] private float _speed = 5;

	 private Rigidbody _rb;
	 private Light _light;
	 private VisualEffect _vfx;
	 private Collider _coll;

	 private void Awake()
	 {
		_light = GetComponentInChildren<Light>();
		_vfx = GetComponentInChildren<VisualEffect>();
	 }

	 private void OnEnable()
	 {
		_vfx.gameObject.SetActive(false);
		_light.intensity = 0;
		_rb = GetComponent<Rigidbody>();
		_rb.velocity = Vector3.zero;
		_coll = GetComponent<Collider>();

		_laser.maxLength = 0;
		DOTween.To(() => _laser.maxLength, (x) => _laser.maxLength = x, 1, 0.2f).OnComplete(() => {
		  _rb.velocity = transform.forward * _speed;
		});
		_light.DOIntensity(1, .7f);

	 }

	 private void OnCollisionEnter(Collision collision)
	 {
		if (_coll.Equals(collision.collider))
		  return;

		if (Player.PlayerBehaviour.IsPlayerCollider(collision.collider))
		  Player.PlayerBehaviour.Damage(25f);

		_rb.velocity = Vector3.zero;
		_rb.isKinematic = true;
		GetComponent<CapsuleCollider>().enabled = false;

		_vfx.transform.position = collision.contacts[0].point;

		_vfx.gameObject.SetActive(true);
		_light.DOIntensity(1.4f, 0.15f).OnComplete(() => {
		  _light.DOIntensity(0, 0.5f);
		});
		_laser.gameObject.SetActive(false);
		Destroy(gameObject, 5);
	 }
  }
}