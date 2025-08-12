using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using DG.Tweening;

namespace it.Game.NPC.Enemyes.Boses.Karmamancer
{
  public class MiniBullet : MonoBehaviourBase
  {
	 [SerializeField]
	 private GameObject _hitPrefab;
	 private float _bulletSpeed = 20;
	 private float _angleChangeSpeed = 15f;
	 private Rigidbody _rb;
	 private Transform _source;
	 private Transform targetObject;
	 private bool _isBullet = false;

	 private float _shootTime;

	 public UnityEngine.Events.UnityAction<Transform> onDamage;
	 
	 private Vector3 Target => targetObject.position + Vector3.up;

	 private bool _isContact;

	 private void Start()
	 {

		GetComponent<VisualEffect>().SendEvent("OnBase");
	 }

	 public void Shoot(Transform source, Transform target)
	 {
		_isContact = false;
		_source = source;
		this.targetObject = target;
		transform.parent = null;
		transform.rotation = Quaternion.LookRotation(Target - transform.position);
		_rb = GetComponent<Rigidbody>();
		GetComponent<VisualEffect>().SendEvent("OnTrail");
		_isBullet = true;
		_shootTime = Time.time;
	 }

	 private void Update()
	 {
		if (!_isBullet)  return;
		if (_isContact) return;

		if(_shootTime > Time.time - 2)
		  transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Target - transform.position), _angleChangeSpeed * Time.deltaTime);
		_rb.velocity = transform.forward * _bulletSpeed;

		if (_shootTime < Time.time - 20)
		  Contact(transform);
	 }


	 private void OnCollisionEnter(Collision collision)
	 {
		if (!_isBullet)
		  return;
		Contact(collision.transform);
	 }

	 private void Contact(Transform tr)
	 {
		if (_isContact)
		  return;

		_isContact = true;
		GetComponent<VisualEffect>().SendEvent("OffTrail");
		GetComponent<Collider>().enabled = false;
		onDamage?.Invoke(tr);
		SpawnHit();
		DestroyThis();
	 }

	 private void SpawnHit()
	 {
		GameObject inst = Instantiate(_hitPrefab);
		inst.transform.position = transform.position;
		inst.gameObject.SetActive(true);
		Destroy(inst, 5);
	 }

	 private void DestroyThis()
	 {
		GetComponent<VisualEffect>().SendEvent("OffBase");
		_rb.velocity = Vector3.zero;
		DOVirtual.DelayedCall(10, () =>
		{
		  Destroy(gameObject);
		});
	 }

  }
}