using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class SarDayyniBullet1Element : MonoBehaviourBase
  {
	 private Rigidbody _rb;
	 [SerializeField]
	 private int _minForce = 1;
	 [SerializeField]
	 private int _maxForce = 3;

	 [SerializeField]
	 private SarDayyniBullet1Damager _damager;
	 private void OnEnable()
	 {
		_damager.gameObject.SetActive(false);
		Vector3 vector = Random.insideUnitSphere;
		vector.y = 0;

		transform.LookAt(transform.position + vector);
		_rb = GetComponent<Rigidbody>();
		_rb.AddForce(transform.forward * Random.Range(_minForce, _maxForce), ForceMode.Impulse);
	 }

	 private void OnCollisionEnter(Collision collision)
	 {
		_damager.transform.parent = transform.parent;
		_damager.gameObject.SetActive(true);
		Destroy(gameObject);
	 }
  }
}