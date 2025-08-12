using UnityEngine;
using com.ootii.Helpers;
using com.ootii.Geometry;
using com.ootii.Timing;
using com.ootii.Actors;


namespace it.Game.NPC.Motions
{
  public class TavrosAttack : AnimatorDriver
  {
	 [SerializeField]
	 private Transform _target;

	 private float _actualSpeed = 0;

	 private float _angleMinLock = 1f;

	 public float _PathHeight = 0.05f;

	 public float PathHeight
	 {
		get { return _PathHeight; }
		set { _PathHeight = value; }
	 }

	 private bool _attack = false;
	 private float _attackLast = 0;
	 private float _attackInertion = 0;

	 private bool IsInertion => _attackInertion > Time.time;

	 private int State;

	 protected override void Awake()
	 {
		base.Awake();

		mInputSource = null;
	 }

	 protected override void Update()
	 {
		if (!_IsEnabled) { return; }
		if (mActorController == null) { return; }

		// Simulated input for the animator
		Vector3 lMovement = Vector3.zero;
		Quaternion lRotation = Quaternion.identity;
		float yawAngle = 0;

		CalculateMove(_target.position, ref lMovement, ref lRotation, ref yawAngle);


		//mActorController.Move(lMovement);
		//mActorController.Rotate(lRotation);

		SetAnimator(Vector3.zero, lMovement, yawAngle);
	 }

	 protected virtual void CalculateMove(Vector3 rWaypoint, ref Vector3 rMove, ref Quaternion rRotate, ref float lYawAngle)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Direction we need to travel in
		Vector3 lDirection = rWaypoint - transform.position;
		lDirection.y = lDirection.y - _PathHeight;
		lDirection.Normalize();

		// Determine our rotation
		Vector3 lVerticalDirection = Vector3.Project(lDirection, transform.up);
		Vector3 lLateralDirection = lDirection - lVerticalDirection;

		lYawAngle = Vector3Ext.SignedAngle(transform.forward, lLateralDirection);

		if (Mathf.Abs(lYawAngle) < 10 && (rWaypoint - transform.position).magnitude < 3 && Time.time - _attackLast > 1f)
		{
		  _attack = true;
		  _attackLast = Time.time;
		  _attackInertion = _attackLast + Random.Range(1f, 3f);
		}

		if (Mathf.Abs(lYawAngle) < 20)
		{
		  _actualSpeed += Time.deltaTime;
		}
		else if (!IsInertion)
		{
		  _actualSpeed -= Time.deltaTime;
		}

		_actualSpeed = Mathf.Clamp(_actualSpeed, 0, 1);

		if (IsInertion)
		  lYawAngle = 0;

		if (Mathf.Abs(lYawAngle) < _angleMinLock)
		  lYawAngle = 0;
		else if (Mathf.Abs(lYawAngle) < 10)
		  lYawAngle /= 10;
		else
		  lYawAngle = Mathf.Sign(lYawAngle);

		//if (_RotationSpeed == 0f)
		//{
		//  rRotate = Quaternion.AngleAxis(lYawAngle, transform.up);
		//}
		//else
		//{
		//  rRotate = Quaternion.AngleAxis(Mathf.Sign(lYawAngle) * Mathf.Min(Mathf.Abs(lYawAngle), _RotationSpeed * lDeltaTime), transform.up);
		//}

		//  // Grab the base movement speed
		//  float lMoveSpeed = mRootMotionMovement.magnitude / lDeltaTime;
		//  if (lMoveSpeed == 0f) { lMoveSpeed = _MovementSpeed; }

		//  // Calculate our own slowing
		//  float lRelativeMoveSpeed = 1f;

		//  // Set the final velocity based on the future rotation
		//  Quaternion lFutureRotation = transform.rotation * rRotate;
		//rMove = lFutureRotation.Forward()* _actualSpeed;
		//rMove = lFutureRotation.Forward() * (lMoveSpeed * lRelativeMoveSpeed * lDeltaTime);

	 }
	 protected override void OnAnimatorMove()
	 {
		if (Time.deltaTime == 0f) { return; }
		transform.position += mAnimator.deltaPosition;
		//transform.rotation = mAnimator.bodyRotation;m
  mActorController.Rotate(mAnimator.deltaRotation);



		// Clear any root motion values
		//if (mAnimator == null)
		//{
		//  mRootMotionMovement = Vector3.zero;
		//  mRootMotionRotation = Quaternion.identity;
		//}
		//// Store the root motion as a velocity per second. We also
		//// want to keep it relative to the avatar's forward vector (for now).
		//// Use Time.deltaTime to create an accurate velocity (as opposed to Time.fixedDeltaTime).
		//else
		//{
		//  // Convert the movement to relative the current rotation
		//  mRootMotionMovement = Quaternion.Inverse(transform.rotation) * (mAnimator.deltaPosition);

		//  // Store the rotation as a velocity per second.
		//  mRootMotionRotation = mAnimator.deltaRotation;
		//}
	 }

	 protected void SetAnimator(Vector3 rInput, Vector3 rMovement, float rRotation)
	 {
		float lDeltaTime = TimeManager.SmoothedDeltaTime;

		// Determine the simulated input
		if (rMovement.sqrMagnitude > 0f)
		{
		  float lSpeed = 1f;
		  if (_MovementSpeed > 0f) { lSpeed = (rMovement.magnitude / lDeltaTime) / _MovementSpeed; }

		  rInput = Vector3.forward * lSpeed;
		}

		// Tell the animator what to do next
		//mAnimator.SetFloat("Speed", rInput.magnitude);
		//mAnimator.SetFloat("Direction", Mathf.Atan2(rInput.x, rInput.z) * 180.0f / 3.14159f);

		if (_attack)
		  mAnimator.SetTrigger("Attack");
		_attack = false;

		mAnimator.SetFloat("Speed", _actualSpeed);
		mAnimator.SetFloat("Direction", rRotation);


		//mAnimator.SetBool("Jump", false);
	 }

  }
}