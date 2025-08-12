using UnityEngine;

namespace itra.Animations
{
	public abstract class AnimationBase :MonoBehaviour, IAnimation
	{
		[SerializeField] protected bool _isPlaying;
		public virtual bool IsPlaying => _isPlaying;

		public virtual bool Play()
		{
			if (_isPlaying)
				return false;
			_isPlaying = true;
			return true;
		}

		public virtual bool Stop()
		{
			if (!_isPlaying)
				return false;
			_isPlaying = false;
			return true;
		}
	}
}
