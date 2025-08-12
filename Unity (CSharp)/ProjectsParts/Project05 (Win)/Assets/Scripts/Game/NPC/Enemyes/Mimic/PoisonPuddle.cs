using UnityEngine;

using Utilites.Geometry;
using DG.Tweening;

namespace it.Game.NPC.Enemyes
{
  public class PoisonPuddle : MonoBehaviourBase
  {
	 [SerializeField]
	 public GameObject _trigger;
	 private bool isPlayer;

	 private float _lastTime;

	 private void OnEnable()
	 {
		SetGround();

		_lastTime = Time.time;

		ParticleEmission(true);

		DOVirtual.DelayedCall(2f, () =>	{
		  ParticleEmission(false);
		});
		DOVirtual.DelayedCall(3f, () =>	{
		  _trigger.SetActive(false);
		});

		Destroy(gameObject, 10);
	 }

	 public void PlayerEnter()
	 {
		isPlayer = true;
	 }

	 public void PlayerExit()
	 {
		isPlayer = false;
	 }

	 private void Update()
	 {
		if (!isPlayer)
		  return;

		if (_lastTime + 0.5f < Time.time)
		{
		  Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,5, false);
		  _lastTime = Time.time;
		}
	 }

	 private void ParticleEmission(bool isActive)
	 {
		ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

		for (int i = 0; i < particles.Length; i++)
		{
		  var emis = particles[i].emission;
		  emis.enabled = isActive;
		}
	 }

	 public void SetGround()
	 {
		Vector3 startPoint = transform.position;
		RaycastHit[] hits;
		int count = RaycastExt.SafeRaycastAll(transform.position + Vector3.up * 10 / 3, Vector3.down, out hits, 10 * 1.3f, ProjectSettings.GroundAndClimbLayerMaks, transform);

		if (count <= 0)
		  return;

		Vector3 positionTarget = hits[0].point;

		transform.position = positionTarget;

	 }

  }
}