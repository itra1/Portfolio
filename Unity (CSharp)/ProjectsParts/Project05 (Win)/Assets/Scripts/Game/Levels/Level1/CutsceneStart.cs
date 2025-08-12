using it.Game.Managers;
using System.Collections;
using UnityEngine;

namespace it.Game.Levels.Level1
{
	public class CutsceneStart : MonoBehaviour
	{

		public void Play()
		{
			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			bool isWait = false;

			// Анимациясмены кадров
			isWait = true;
			it.Cartoons.Cartoon.Play("15cdbe36-e367-4861-a842-e1ef79f5387f", true, false, () =>	{	isWait = false;	});

			while (isWait) yield return null;

			// Диалог пробуждения
			isWait = true;
			GameManager.Instance.DialogsManager.ShowDialog("4a9f407d-89f9-4f71-9484-2a57c282b8ac", null, null, () =>	 {	isWait = false;	});

			while (isWait) yield return null;

			// Тутор перемещения
			isWait = true;
			it.Game.Managers.TutorialManager.Instance.ShowTutorial(0, () => { isWait = false; });

			while (isWait) yield return null;

			// Тутор медленного перемещения
			isWait = true;
			it.Game.Managers.TutorialManager.Instance.ShowTutorial(1, () => { isWait = false; });

			while (isWait) yield return null;

		}

	}

}