using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Environment.All
{
  public class FlyingRoundAndAnim : Environment
  {
	 private Animator _animator;
	 [SerializeField]
	 private RangeFloat _speedsAnim;

	 [SerializeField]
	 private float _radiusMove = 1;
	 [SerializeField]
	 private RangeFloat _speedMoves;

	 private float _speedMove;
	 private Vector3 _velocity;

	 //private Rigidbody _rb;
	 private Vector3 _startPosition;
	 
	 private Vector3 _targetPosition;

	 protected override void Start()
	 {
		base.Start();
		_startPosition = transform.position;

		_animator = GetComponent<Animator>();
		if(_animator == null)
		  _animator = GetComponentInChildren<Animator>();

		//if (_rb == null)
		//  _rb = gameObject.AddComponent<Rigidbody>();

		//_rb.freezeRotation = true;

		_animator.SetFloat("Anim", Random.value);
		_animator.SetFloat("Speed", _speedsAnim.RandomRange);
		_speedMove = _speedMoves.RandomRange;
		transform.position += Vector3.one * _radiusMove;
		CalcPositionMove(false);
	 }

	 private void Update()
	 {
		_velocity += (_targetPosition - transform.position).normalized * Time.deltaTime*1f;
		_velocity.Normalize();
		//_rb.velocity = _velocity;
		transform.position += _velocity * _speedMove * Time.deltaTime;

		if ((_targetPosition - transform.position).magnitude < _radiusMove/8)
		  CalcPositionMove();
	 }

	 private void CalcPositionMove(bool check = true)
	 {
		do
		{
		  _targetPosition = _startPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * _radiusMove;
		} while (check && (_targetPosition - transform.position).magnitude < _radiusMove*0.7f);

		//_force = ((_startPosition - transform.position) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized * _speedMove;
	 }

	 private void OnDrawGizmosSelected()
	 {
		Gizmos.DrawWireSphere(transform.position, _radiusMove);
		if (Application.isPlaying)
		{
		  Gizmos.DrawWireSphere(_startPosition, 0.01f);
		  Gizmos.color = Color.red;
		  Gizmos.DrawWireSphere(_targetPosition, 0.01f);
		}
	 }

  }
}