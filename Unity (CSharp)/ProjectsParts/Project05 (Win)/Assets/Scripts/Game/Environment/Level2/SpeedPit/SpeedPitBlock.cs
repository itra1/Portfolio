using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace it.Game.Environment.Level2.SpeedPit
{
  /// <summary>
  /// Блок
  /// </summary>
  public class SpeedPitBlock : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _block;
	 [SerializeField]
	 private ParticleSystem _particles;

	 private void Start()
	 {
		_block.transform.localPosition = Vector3.up * -3;
	 }

	 public void SetVisibleStone(float timeActive = 2)
	 {
		_block.DOLocalMoveY(0, 0.2f);
		_particles.Play();
		PlayStoneSound();
		DOVirtual.DelayedCall(timeActive, () => {
		  _block.DOLocalMoveY(-3, 0.2f);
		  _particles.Play();
		  PlayStoneSound();
		},false);
	 }

	 private void PlayStoneSound()
	 {
		DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtVector3("Effects", transform.position, 1, null, 0, "StoneMoveShort" + Random.Range(1, 6));
		 //DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransform("Effects", transform, 1, null, 0, "Sound/Effects/StoneMove/BlockMoveShort" + Random.Range(1,6));
	 }

	 private void OnDrawGizmosSelected()
	 {
		//if(_particles == null)
		//{
		//  _particles = GetComponentInChildren<ParticleSystem>();
		//  _particles.transform.localPosition = Vector3.zero;
		//}

	 }
  }
}