using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Environment.Handlers;

namespace it.Game.Environment.Level1
{
	public class ScientistVisible : Environment
	{
		[SerializeField] private PlayMakerFSM _fsmEnemy;
		[SerializeField] private ParticleSystem _crystalPsrticles;
		[SerializeField] private GameObject _crystal;
		[SerializeField] private Light _crystalLight;

		protected override void Start()
		{
			base.Start();

			FirstInit();
		}

		private void FirstInit()
		{
			_crystalPsrticles.gameObject.SetActive(true);
			_crystal.gameObject.SetActive(true);
			_crystalLight.gameObject.SetActive(true);
			// _enemy.gameObject.SetActive(false);
			_crystalLight.intensity = 3;
		}

		protected override void ConfirmState(bool isForce = false)
		{
			base.ConfirmState(isForce);

			if (State == 2)
			{
				if (isForce)
				{
					_crystalPsrticles.gameObject.SetActive(false);
					_crystal.gameObject.SetActive(false);
					_crystalLight.gameObject.SetActive(false);
					//_enemy.gameObject.SetActive(false);
				}
				else
				{
					_crystalLight.DOIntensity(0, 0.2f);
					_crystalPsrticles.Play();
					_crystal.gameObject.SetActive(false);
					//_enemy.gameObject.SetActive(true);
					_fsmEnemy.FsmVariables.GetFsmGameObject("Player").Value = it.Game.Player.PlayerBehaviour.Instance.gameObject;
					_fsmEnemy.Fsm.Event("StartFSM");
				}
			}
			else
			{
				FirstInit();
			}

		}

		public void Activate()
		{
			if (State == 2)
				return;
			State = 2;
			Save();
			ConfirmState();
		}

	}
}