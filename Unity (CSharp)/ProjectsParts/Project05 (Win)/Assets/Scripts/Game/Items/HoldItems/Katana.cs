using UnityEngine;
using System.Collections;
using UnityEditor;
namespace it.Game.Items
{
  public class Katana : HoldenItem
  {
	 public override int HoldStey => 1;
	 [SerializeField]
	 private Renderer _material;

	 [SerializeField]
	 private Color _colorDisactive;
	 [SerializeField]
	 private Color _colorActive;

	 [SerializeField]
	 private Collider _damageCollider;
	 private float _timeStartUse;
	 private bool _isDamage = false;
	 private bool _isContantReady = false;

	 public void Activate()
	 {
		_material.material.SetColor("_EmissionColor", _colorActive);
	 }

	 public override void Use()
	 {
		base.Use();
		_timeStartUse = Time.time;
		_isDamage = false;
		_damageCollider.enabled = true;
	 }
	 public override void UnUse()
	 {
		base.Use();
		_damageCollider.enabled = false;
	 }

	 public void SetAttackStart()
	 {
		_isDamage = false;
		_isContantReady = true;
	 }

	 public void SetAttackStop()
	 {
		_isContantReady = false;
	 }

	 public override void Drop()
	 {
		base.Drop();
		_damageCollider.enabled = false;
	 }

	 public void OnTriggerEnter(Collider other)
	 {
		if (_timeStartUse + 0.15f > Time.time)
		  return;

		var mob = other.transform.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer>();
		if (mob != null)
		{
		  if (!_isDamage)
		  {
			 _isDamage = true;
			 mob.Damage();
		  }
		}
	 }

	 public void CatanaTriggerEnter(Collider other)
	 {
		if (_isContantReady) return;

		var mob = other.transform.GetComponent<it.Game.NPC.Enemyes.Boses.Karmamancer.TheKarmamancer>();
		if (mob != null)
		{
		  if (!_isDamage)
		  {
			 _isDamage = true;
			 mob.Damage();
		  }
		}
	 }
  }
}