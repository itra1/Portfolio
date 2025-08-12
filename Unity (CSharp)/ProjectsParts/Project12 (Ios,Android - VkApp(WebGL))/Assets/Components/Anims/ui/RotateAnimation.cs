using Cysharp.Threading.Tasks;
using UnityEngine;

namespace itra.Animations.UI
{
	public class RotateAnimation : AnimationUniTask
	{
		[SerializeField] private float _rotationSpeed = -1;
		[SerializeField] private Vector3 _rotationAxis = Vector3.forward;

		protected override UniTask UpdateAnimation()
		{
			transform.rotation *= Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, _rotationAxis);
			return default;
		}
	}
}
