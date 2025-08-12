using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace it.Game.NPC.Enemyes
{
  public class LavaGolemBullet : MonoBehaviour
  {
	 private SphereCollider _sphereCollider;
	 private Rigidbody _rb;
	 private ParticleSystem _particles;
	 private ParticleSystem.EmissionModule _emission;
	 private Transform _meshTransform;

	 private void Awake()
	 {
		_rb = GetComponent<Rigidbody>();
		_sphereCollider = GetComponent<SphereCollider>();
		_particles = GetComponentInChildren<ParticleSystem>();
		_emission = _particles.emission;
		_emission.enabled = false;
		_rb.isKinematic = true;
		_rb.velocity = Vector3.zero;
		_rb.useGravity = false;
		_sphereCollider.enabled = false;
		_rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		_rb.interpolation = RigidbodyInterpolation.None;
		_meshTransform = _rb.GetComponentInChildren<Renderer>().transform;
	 }

	 private void Start()
	 {
		_meshTransform.localScale = Vector3.zero;
		_meshTransform.DOScale(1, 0.5f);
	 }

	 public void Shoot()
	 {
		_sphereCollider.enabled = true;
		_emission.enabled = true;
		_rb.isKinematic = false;
		_rb.useGravity = true;
		_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
		_rb.interpolation = RigidbodyInterpolation.Interpolate;
		_rb.AddForce(((Player.PlayerBehaviour.Instance.transform.position + Vector3.up*1.7f) - transform.position).normalized * 700);
	 }

	 private void OnCollisionEnter(Collision collision)
	 {
		if (Player.PlayerBehaviour.IsPlayerCollider(collision.collider))
		  Player.PlayerBehaviour.Damage(25f);

		SpawnSubParticles();
		Collider c = GetComponent<Collider>();
		c.enabled = false;
		_meshTransform.gameObject.SetActive(false);
		_emission.enabled = false;
		Destroy(_rb.gameObject, 5);
	 }

	 private void SpawnSubParticles()
	 {
		int subCount = Random.Range(3, 10);

		for(int i = 0;i < subCount; i++)
		{
		  GameObject inst = new GameObject();
		  inst.transform.position = transform.position;
		  inst.transform.localScale = Vector3.one * Random.Range(0.25f,0.4f);
		  Rigidbody rb = inst.AddComponent<Rigidbody>();
		  rb.isKinematic = false;
		  rb.useGravity = true;
		  GameObject part = Instantiate(_particles.gameObject);
		  part.transform.SetParent(inst.transform);
		  part.transform.localPosition = Vector3.zero;
		  part.transform.localScale = Vector3.one;
		  Vector3 moveVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
		  moveVector.Normalize();
		  moveVector.y = Random.Range(0, 1.5f);
		  rb.AddForce(moveVector * Random.Range(200f, 400f));
		  var mod = part.GetComponent<ParticleSystem>().inheritVelocity;
		  mod.enabled = false;
		  Destroy(inst, 10);
		}

	 }

  }
}