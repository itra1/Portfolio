using it.Game.Managers;
using System.Collections;
using UnityEngine;
using com.ootii.Cameras;

namespace it.Game.Levels.Level1
{
	public class CutsceneDungeonOut : MonoBehaviour
	{
		[SerializeField] private GameObject _note;
		[SerializeField] private Transform _cameraPosition;
		[SerializeField] private Animator _budka;

		[ContextMenu("Play")]
		public void Play()
		{
			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			GameManager.Instance.GameInputSource.IsEnabled = false;
			_note.SetActive(true);

			CameraController cc = CameraBehaviour.Instance.GetComponent<CameraController>();

			var motion = cc.GetMotor<CopyTransformFromTarget>("FixedPosition");
			motion.Target = _cameraPosition;
			cc.ActivateMotor(motion);

			yield return new WaitForSeconds(1);

			_budka.SetBool("Open", true);

			yield return new WaitForSeconds(5);

			GameManager.Instance.GameInputSource.IsEnabled = true;
			var motionGame = cc.GetMotor<it.Game.Gamera.Motors.OrbitFollowMotor>("Game");
			cc.ActivateMotor(motionGame);
		}

		[ContextMenu("Complete")]
		public void Complete()
		{
			_budka.SetBool("Open", true);
			_note.SetActive(true);
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			_budka.SetBool("Open", false);
			_note.SetActive(false);

		}
	}
}