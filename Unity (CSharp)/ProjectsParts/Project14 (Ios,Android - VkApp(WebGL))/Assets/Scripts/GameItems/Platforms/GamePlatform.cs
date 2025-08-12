using Core.Engine.Elements.Player;
using DG.Tweening;
using SoundPoint;
using UnityEngine;
using Zenject;

namespace Scripts.GameItems.Platforms
{
	public class GamePlatform :Platform, IPlayerCrossing
	{
		[SerializeField] private PlatformFormations _formations;
		[SerializeField] private AudioClip[] _destroyClips;

		private static int _lastDestroyClipIndex = -1;

		private MaterialPropertyBlock _materialProperty;
		private IAudioPointFactory _audioPointFactory;
		private float _opacity;

		[Inject]
		public void Constructor(IAudioPointFactory audioPointFactory)
		{
			_audioPointFactory = audioPointFactory;
		}

		private void DestroyClipSound()
		{

			if (_destroyClips.Length == 0)
				return;

			int indexClip = _lastDestroyClipIndex;
			while (_destroyClips.Length > 2 && indexClip == _lastDestroyClipIndex)
				indexClip = Random.Range(0, _destroyClips.Length);

			_ = _audioPointFactory.Create().AutoDespawn().Play(_destroyClips[indexClip]);

			_lastDestroyClipIndex = indexClip;
		}

		public void PlayerCrossing()
		{
			_opacity = 1;
			_materialProperty = new();
			_materialProperty.SetFloat("_Opacity", _opacity);

			DestroyClipSound();

			for (var i = 0; i < _formations.ActiveItems.Count; i++)
			{

				var itm = _formations.ActiveItems[i];
				itm.transform.SetParent(null);

				var mCol = itm.GetOrAddComponent<MeshCollider>();
				mCol.enabled = false;

				var rb = itm.GetOrAddComponent<Rigidbody>();
				rb.useGravity = true;
				rb.isKinematic = false;

				Vector3 forceVector = -itm.transform.up;

				if (Vector3.Angle(Vector3.back, forceVector) < 90 && Vector3.Dot(Vector3.back, forceVector) > 0)
				{
					forceVector = Vector3.SignedAngle(Vector3.back, forceVector, Vector3.up) < 0
					? Vector3.right
					: Vector3.left;
				}

				rb.AddForce(forceVector * 4, ForceMode.Impulse);
				rb.AddTorque(new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), ForceMode.Impulse);

				var rnd = itm.GetOrAddComponent<Renderer>();
				rnd.SetPropertyBlock(_materialProperty);
			}

			var tt = DOTween.To(() => _opacity, (x) => _opacity = x, 0, 1f);
			_ = tt.OnComplete(() =>
			{
				for (var i = 0; i < _formations.ActiveItems.Count; i++)
					Destroy(_formations.ActiveItems[i].gameObject);
				_formations.ActiveItems.Clear();
				gameObject.SetActive(false);
			});
			_ = tt.OnUpdate(() =>
			{
				_opacity = 1 - tt.position;
				_materialProperty.SetFloat("_Opacity", _opacity);
				for (var i = 0; i < _formations.ActiveItems.Count; i++)
				{
					var itm = _formations.ActiveItems[i];
					var rnd = itm.GetOrAddComponent<Renderer>();
					rnd.SetPropertyBlock(_materialProperty);
				}
			});

			DestroyEvent?.Invoke();
			DestroyEvent.RemoveAllListeners();
		}

		public override void ResetFormation()
		{

			_opacity = 1;

			for (var i = 0; i < _formations.ActiveItems.Count; i++)
			{

				var itm = _formations.ActiveItems[i];
				itm.GetOrAddComponent<Collider>().enabled = true;

				var rb = itm.GetOrAddComponent<Rigidbody>();
				rb.useGravity = false;
				rb.isKinematic = true;
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;

				var rnd = itm.GetOrAddComponent<Renderer>();
				_materialProperty = new();
				_materialProperty.SetFloat("_Opacity", _opacity);

				rnd.SetPropertyBlock(_materialProperty);
			}
		}
	}
}
