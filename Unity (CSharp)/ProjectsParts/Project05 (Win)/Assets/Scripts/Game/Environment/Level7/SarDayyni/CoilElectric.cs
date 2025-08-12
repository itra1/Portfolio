using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.VFX;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class CoilElectric : MonoBehaviour
  {
	 public System.Action OnActivate;

	 [SerializeField] private Renderer _privod;
	 [ColorUsage(false, true)]
	 [SerializeField] private Color _colorActive;
	 [ColorUsage(false, true)]
	 [SerializeField] private Color _middleColor;

	 private CoilElectricElement[] _cails;
	 public CoilElectricElement[] Cails { get => _cails; set => _cails = value; }

	 public bool IsActivated
	 {
		get
		{
		  for (int i = 0; i < _cails.Length; i++)
			 if (!_cails[i].IsActivated)
				return false;
		  return true;
		}
	 }


	 private void Awake()
	 {
		_cails = GetComponentsInChildren<CoilElectricElement>();
		_privod.material = Instantiate(_privod.material);

		for (int i = 0; i < _cails.Length; i++)
		  _cails[i].OnActivate = ElementActivate;
	 }

	 private void Start()
	 {
		Clear();
	 }

	 public void Clear()
	 {
		foreach (var item in _cails) item.Clear();

		_privod.material.SetColor("_EmissionColor", _colorActive);
		ColorChange(0);
	 }

	 private void ElementActivate()
	 {
		int countActive = 0;
		for (int i = 0; i < _cails.Length; i++)
		  if (_cails[i].IsActivated)
			 countActive++;

		ColorChange(countActive);

		if (countActive == _cails.Length)
		  OnActivate?.Invoke();
	 }

	 private void ColorChange(int count)
	 {
		switch (count)
		{
		  case 2:
			 _privod.material.DOColor(Color.black, "_EmissionColor", 1);
			 break;
		  case 1:
			 _privod.material.DOColor(_middleColor, "_EmissionColor", 1);
			 break;
		  case 0:
		  default:
			 _privod.material.SetColor("_EmissionColor", _colorActive);
			 break;
		}
	 }

  }
}