using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.All.BarrierGate
{
	public class BarrierGate : Environment
	{
		/*
		 * Статусы: 
		 * 0 - закрыт
		 * 1 - открыт
		 */

		[SerializeField] private Renderer _shield;

		[ContextMenu("OpenGate")]
		public void OpenGate()
		{
			if (State > 0)
				return;

			GateOpenAnimate();

		State = 1;
			Save();
		}

		[ContextMenu("Gate Open Animate")]
		private void GateOpenAnimate(){
			_shield.gameObject.SetActive(true);
			Material mat = _shield.material;
			mat.SetFloat("_Dissolve", 1);
			mat.DOFloat(0, "_Dissolve", 3).OnComplete(()=> {
				_shield.gameObject.SetActive(false);
			});

			DarkTonic.MasterAudio.MasterAudio.PlaySound("Effects",
			 1,
			 null,
			 0,
			 "OpenEnergyGate");
		}

		[ContextMenu("Gate Close")]
		private void GateClose()
		{
			_shield.gameObject.SetActive(true);
			Material mat = _shield.material;
			mat.SetFloat("_Dissolve", 1);
		}

		[ContextMenu("Gate Open")]
		private void GateOpen()
		{
			_shield.gameObject.SetActive(false);
		}

		protected override void ConfirmState(bool isForce = false)
		{
			base.ConfirmState(isForce);

			if (State <= 0)
			{
				GateClose();
			}
			else
			{
				GateOpen();
			}

		}


	}
}