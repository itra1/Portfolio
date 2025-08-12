using System.Collections;

using UnityEngine;

namespace it.Game.Environment.Level5
{
  public class DungeonBossArena : MonoBehaviour
  {
	 [SerializeField] private BossArenaDoor _frontDoor;
	 [SerializeField] private BossArenaDoor _leftDoor;
	 [SerializeField] private BossArenaDoor _rightDoor;

	 public void AnimateOpenLeft(bool isOpen)
	 {
		OpenLeft(isOpen, true);
	 }
	 public void OpenLeft(bool isOpen, bool animate = false)
	 {
		if (isOpen)
		  _leftDoor.Open(animate);
		else
		  _leftDoor.Close(animate);
	 }

	 public void AnimateOpenRight(bool isOpen)
	 {
		OpenRight(isOpen, true);
	 }
	 public void OpenRight(bool isOpen, bool animate = false)
	 {
		if (isOpen)
		  _rightDoor.Open(animate);
		else
		  _rightDoor.Close(animate);
	 }

	 public void AnimateOpenFront(bool isOpen)
	 {
		OpenFront(isOpen, true);
	 }
	 public void OpenFront(bool isOpen, bool animate = false)
	 {
		if (isOpen)
		  _frontDoor.Open(animate);
		else
		  _frontDoor.Close(animate);
	 }


	 private void OnDrawGizmos()
	 {
		Gizmos.DrawIcon(transform.position, "StarIcon");
	 }

  }
}