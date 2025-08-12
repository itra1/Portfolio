using UnityEngine;
using System.Collections;

namespace it.Game.NPC.Enemyes.Boses.Priest
{
  public class Attack1Bullet : MonoBehaviourBase
  {

	 private float _bulletSpeed = 10;
	 private float _angleChangeSpeed = 15f;
	 private Rigidbody _rb;
	 private Transform _source;
	 private Transform targetObject;
	 private bool _isBullet = false;
	 [SerializeField]
	 private float _damageValue;

	 public UnityEngine.Events.UnityAction<bool> onDamage;

	 private Vector3 Target => targetObject.position + Vector3.up;

	 public void Shoot(Transform source, Transform target)
	 {
		_source = source;
		this.targetObject = target;
		transform.parent = null;
		transform.rotation = Quaternion.LookRotation(Target - transform.position);
		_rb = GetComponent<Rigidbody>();
		_isBullet = true;
	 }

	 private void Update()
	 {
		if (!_isBullet)
		  return;

		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Target - transform.position), _angleChangeSpeed * Time.deltaTime);
		_rb.velocity = transform.forward * _bulletSpeed;
	 }

	 private void OnCollisionEnter(Collision collision)
	 {
		if (!_isBullet)
		  return;

		bool isSource = CheckCollision(_source, collision);

		if (isSource)
		  return;

		bool isTarget = CheckCollision(targetObject, collision);

		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,_damageValue, true);
		Destroy(gameObject);
	 }

	 private bool CheckCollision(Transform checker, Collision collision)
	 {
		bool isChecker = collision.transform.GetComponent<Transform>().Equals(checker);

		if (!isChecker)
		{
		  Transform[] trs = collision.transform.GetComponentsInParent<Transform>();

		  for (int i = 0; i < trs.Length; i++)
		  {
			 if (!isChecker && trs[i].Equals(checker))
				isChecker = true;
		  }
		}
		return isChecker;

	 }

  }
}