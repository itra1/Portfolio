using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Game.Debugger
{
	/// <summary>
	/// Счетчик кадров
	/// </summary>
	public class FpsCounter : MonoBehaviour
	{
		[SerializeField]
		private Text m_fps;
		private int m_frame = 0;
		private float m_time = 0;

		private void OnEnable()
		{
			ResetCounter();
		}

		private void Update()
		{
			m_frame++;

			if (m_frame >= 30)
			{
				ConfirmText();
				ResetCounter();
			}
		}

		private void ConfirmText()
		{
			//m_fps.text = ((int)(1/((Time.time - m_time) / m_frame))).ToString();
			m_fps.text = ((int)(1/Time.deltaTime)).ToString();
		}

		private void ResetCounter()
		{
			m_frame = 0;
			m_time = Time.time;
		}
	}
}