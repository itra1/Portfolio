using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace it.Game.Panels
{
	/// <summary>
	/// Îפסכחשו ןאםוכü
	/// </summary>
	public class JackpotPanel : MonoBehaviour
	{
		[SerializeField] private JackpotRoll[] _rolls;
		[SerializeField] private RectTransform _body;

		private CanvasGroup _cg;
		private Animator _animator;

		private void OnEnable()
		{
			if (_cg == null)
				_cg = GetComponent<CanvasGroup>();

			//if (_animator == null)
			//	_animator = GetComponent<Animator>();

			//_animator.SetTrigger("visible");

			StartCoroutine(VisibleAnimations());

			//float value = Mathf.Round(Random.Range(0, 100));

			//SetValue(2.29m);
		}

		IEnumerator VisibleAnimations(){


			if (_animator == null)
				_animator = GetComponent<Animator>();
			_animator.enabled = true;

			_animator.SetTrigger("visible");

			yield return new WaitForSeconds(4);

			_animator.enabled = false;
			_body.DOScale(Vector2.zero, 0.3f).OnComplete(() =>
			{
				gameObject.SetActive(false);
			});
		}

		public void SetValue(decimal value)
		{
			string strValue = value.ToString();
			int valueLenght = strValue.Length;

			if(valueLenght > 5){
				strValue = ((int)value).ToString();
				valueLenght = strValue.Length;
			}

			int index = 0;

			if(valueLenght <= _rolls.Length-3){
				_rolls[index].SetClear();
					index++;
			}

			_rolls[index].SetSymbol();
			index++;

			for (int i = 0; index < _rolls.Length; index++, i++){

				if (i < valueLenght)
				{
					string v = strValue.Substring(i, 1);
					_rolls[index].SetData(v);
				}
				else
					_rolls[index].SetClear();
			}

		}

	}
}