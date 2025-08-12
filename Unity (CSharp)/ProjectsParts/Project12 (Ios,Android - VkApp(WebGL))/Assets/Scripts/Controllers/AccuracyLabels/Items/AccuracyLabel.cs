using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Controllers.AccuracyLabels.Items
{
	public abstract class AccuracyLabel : MonoBehaviour
	{
		[SerializeField] private Transform _body;

		public abstract string Accuracy { get; }

		public async UniTask VisibleAnimation()
		{
			_body.localScale = Vector3.zero;
			gameObject.SetActive(true);
			await _body.DOScale(Vector3.one, 0.1f).ToUniTask();
			await UniTask.Delay(200);
			await _body.DOScale(Vector3.zero, 0.1f).ToUniTask();
			gameObject.SetActive(false);
		}
	}
}
