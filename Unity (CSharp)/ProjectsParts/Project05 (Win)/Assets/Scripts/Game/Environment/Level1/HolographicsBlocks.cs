using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level1
{
	public class HolographicsBlocks : Environment
	{
		[SerializeField] private AnimationCurve _animationCurve;
		[SerializeField] private Material _material;
		[SerializeField] private GameObject _blocks;
		[SerializeField]

		private Material[] _materials;
		private Light[] _light;

		protected override void Awake()
		{
			base.Awake();
			State = 0;
			ConfirmState(true);
		}

		public void Activate()
		{

			var pegasus = GetComponentInChildren<it.Game.Environment.Handlers.PegasusController>(true);

			pegasus.Activate(() =>
			{
				VisibleConstruction();
				DG.Tweening.DOVirtual.DelayedCall(3, () =>
				{
					pegasus.Deactivate();
				});
			});

		}

		public void VisibleConstruction()
		{
			_blocks.SetActive(true);
			//DOTween.To(() => _material.GetFloat("_AllVisible"), (x) => _material.SetFloat("_AllVisible", x), 1, 1);
			StartCoroutine(ActivateIenumerator());
			State = 1;
			Save();
		}

		IEnumerator ActivateIenumerator()
		{
			_light = GetComponentsInChildren<Light>();
			float time = 0;
			_blocks.SetActive(true);
			SetMaterialVisible(0);
			SetLight(0);
			//_material.SetFloat("_AllVisible", 0);
			while (time < 1)
			{
				// float val = _animationCurve.Evaluate(time);
				//_blocks.SetActive(val > 0.5f);
				SetMaterialVisible(time);
				SetLight(time);
				//_material.SetFloat("_AllVisible", val);
				yield return null;
				time += Time.deltaTime;
			}
			SetMaterialVisible(1);
			SetLight(1);
			//_material.SetFloat("_AllVisible", 1);
			//_blocks.SetActive(true);
		}

		private void SetMaterialVisible(float val)
		{
			for (int i = 0; i < _materials.Length; i++)
			{
				_materials[i].SetFloat("_Dissolve", val);
			}
		}
		private void SetLight(float val)
		{
			for (int i = 0; i < _materials.Length; i++)
			{
				_light[i].intensity = val * 6;
			}
		}

		protected override void ConfirmState(bool isForce = false)
		{
			base.ConfirmState(isForce);
			_blocks.SetActive(State > 0);
			_blocks.SetActive(State > 0);
			//_material.SetFloat("_AllVisible", (State > 0 ? 1 : 0));
		}
	}
}