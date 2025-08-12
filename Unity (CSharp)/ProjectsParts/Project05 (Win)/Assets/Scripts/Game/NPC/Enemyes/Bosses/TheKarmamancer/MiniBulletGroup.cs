using UnityEngine;
using System.Collections;
namespace it.Game.NPC.Enemyes.Boses.Karmamancer
{
  public class MiniBulletGroup : MonoBehaviourBase
  {
	 [SerializeField]
	 private float _rotateSpeed = 10;
	 private float _degreeBeforeOrb = 120;
	 public float DegreeBeforeOrb => _degreeBeforeOrb;
	 private float _timeBeforeOrb = 2;
	 public float TimeBeforeOrb => _timeBeforeOrb;
	 [SerializeField]
	 private MiniBullet[] _bullets;
	 private int count = 3;

	 public UnityEngine.Events.UnityAction<bool> onComplete;

	 private int index;
	 private bool existsDamage;

	 private int shootCompleteCount;

	 public float FullRotate => (360 / _degreeBeforeOrb) * _timeBeforeOrb;

	 private void Start()
	 {
		transform.localScale = Vector3.one;
		index = -1;
		existsDamage = false;
		shootCompleteCount = 0;
	 }

	 public float RotateSpeed
	 {
		get
		{
		  return _degreeBeforeOrb / _timeBeforeOrb;
		}
	 }

	 [SerializeField]
	 private Transform _rotateObjext;

	 private void Update()
	 {
		_rotateObjext.localRotation *= Quaternion.Euler(0, RotateSpeed * Time.deltaTime, 0);
	 }

	 public void Shoot(Transform source,  Transform target)
	 {
		index++;

		if (index >= _bullets.Length)
		  return;

		_bullets[index].Shoot(source, target);
		_bullets[index].onDamage = (damageTransform) =>
		{
		  bool isPlayer = Player.PlayerHelpers.IsPlayer(damageTransform);

		  if (isPlayer)
		  {
			 Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,34,true);
			 existsDamage = true;
		  }
		  shootCompleteCount++;

		  if(shootCompleteCount == 3)
		  {
			 onComplete?.Invoke(existsDamage);
			 Destroy(gameObject, 1);
		  }
		};
	 }

  }
}