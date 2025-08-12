using UnityEngine;
using System.Collections;
using it.Game.Player;

namespace it.Game.NPC.Enemyes
{
  public class MimicBullet : MonoBehaviourBase
  {
	 [SerializeField]
	 private float _speed = 5f;
	 [SerializeField]
	 private float _gravity = 0.3f;
	 [SerializeField]
	 public ParticleSystem _baseParticle;
	 [SerializeField]
	 public it.Game.NPC.Enemyes.PoisonPuddle _puppleParticle;

	 [SerializeField]
	 private GameObject _hitParticle;

	 [HideInInspector]
	 public Transform _parent;

	 private Vector3 velocity;
	 private Rigidbody _rb;

	 public void Shoot(Transform spawnPoint, Transform parent)
	 {
		GameObject obj = Instantiate(gameObject);
		obj.transform.position = spawnPoint.position;
		obj.transform.LookAt(PlayerBehaviour.Instance.transform.position + Vector3.up * 1.3f, Vector3.up);
		obj.GetComponent<MimicBullet>()._parent = parent;
		obj.SetActive(true);
	 }

	 private void OnEnable()
	 {
		velocity = transform.forward * _speed;
		_rb = GetComponent<Rigidbody>();
	 }


	 private void Update()
	 {
		velocity += Vector3.down * _gravity * Time.deltaTime;
	 }

	 private void FixedUpdate()
	 {
		_rb.velocity = velocity;
	 }

	 private void OnCollisionEnter(Collision collision)
	 {
		Transform[] trs = _parent.GetComponentsInChildren<Transform>();
		for(int i = 0; i < trs.Length; i++)
		{
		  if (trs[i] == collision.transform)
			 return;
		}

		Debug.Log(collision.gameObject.name);
		Hit();
	 }

	 public void OnPlayerContact()
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this, 35, true);
		Hit();
	 }

	 private void Hit()
	 {
		_hitParticle.transform.SetParent(null);
		Destroy(_hitParticle, 1);
		_baseParticle.transform.SetParent(null);
		ParticleEmission(false);
		_puppleParticle.transform.SetParent(null);
		_puppleParticle.gameObject.SetActive(true);
		Destroy(gameObject);
		Destroy(_baseParticle.gameObject,5);
	 }
	 private void ParticleEmission(bool isActive)
	 {

		ParticleSystem[] particles = _baseParticle.GetComponentsInChildren<ParticleSystem>();

		for (int i = 0; i < particles.Length; i++)
		{
		  var emis = particles[i].emission;
		  emis.enabled = isActive;
		}
		if (isActive)
		{
		  _baseParticle.Play();
		}
		else
		{
		  _baseParticle.Stop();
		}
	 }

  }
}