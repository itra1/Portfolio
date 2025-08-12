using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using it.Game.Environment.Handlers;

namespace it.Game.Environment.Challenges.WheelPuzzle
{
  public class WheelPuzzle : Challenge
  {
	 [ContextMenuItem("Confirm", "ConfirmTextCounter")]
	 [SerializeField]
	 private int _maxValue = 100;

	 public UnityEngine.Events.UnityEvent OnRoll;

	 [SerializeField]
	 private Renderer _crystalRenderer;

	 [SerializeField]
	 private Wheel[] _whells;
	 [SerializeField]
	 private Collider[] _baseColliders;
	 [SerializeField]
	 private Collider[] _gameColliders;

	 [SerializeField]
	 private Material _crystal;
	 [SerializeField]
	 private Light _light;

	 [ContextMenuItem("Conform", "ConfirmCrystalColor")]
	 [SerializeField]
	 [ColorUsage(true,true)]
	 private Color _activeCrystal;
	 [SerializeField]
	 [ColorUsage(true, true)]
	 private Color _deactiveCrystal;
	 [ContextMenuItem("Conform", "ConfirmLightColor")]
	 [SerializeField]
	 [ColorUsage(true, true)]
	 private Color _lightCrystal;

	 [ContextMenuItem("Confirm", "ConfirmTextCounter")]
	 [SerializeField]
	 private TextMesh _text;

	 private PhaseType _phase = PhaseType.none;

	 private int _value = 100;
	 public int Value
	 {
		get => _value; set
		{
		  _value = value;
		  SetTest();
		}
	 }
	 private bool _isMove = false;

	 public override bool IsInteractReady => State == 0;
	 public override bool InteractionReady()
	 {
		return IsInteractReady;
	 }

	 [SerializeField]
	 private Transform _cameraFocus;
	 [SerializeField]
	 private Transform _fummaryFocus;
	 [SerializeField]
	 private Transform _intFocus;
	 [SerializeField]
	 private Transform _multiplyFocus;
	 int decrementValue = 0;

	 protected override void Start()
	 {
		base.Start();
		_light.gameObject.SetActive(false);
		ResetValue();
	 }

	 private enum PhaseType
	 {
		inc,
		mult,
		none
	 }

	 public override void StartInteract()
	 {
		if (State != 0)
		  return;

		State = 1;
		_light.intensity = 0;
		_light.gameObject.SetActive(true);

		DOTween.To(() => _text.transform.localScale, x => _text.transform.localScale = x, Vector3.one, 1f);
		DOTween.To(() => _light.intensity, x => _light.intensity = x, 5, 1f);
		DOTween.To(() => _crystal.GetColor("_EmissionColor"), x => _crystal.SetColor("_EmissionColor", x), _activeCrystal, 1f);
		base.StartInteract();
		_phase = PhaseType.none;
		FocusCamera(_cameraFocus, 0.5f);
		Move(_fummaryFocus, 0.001f, 0f);
		Move(_intFocus, 0.5f, 1,()=> {
		  _phase = PhaseType.inc;
		});

		_mouseHandler.onMouseDown = MouseDown;

	 }

	 private void MouseDown()
	 {

		if (_isMove)
		  return;
		RaycastHit[] hits = _mouseHandler.GetMouseRayHits(10f);
		if (hits.Length == null)
		  return;
		for (int i = 0; i < hits.Length; i++)
		{

		  if (hits[i].collider == null)
			 continue;
		  Wheel well = hits[i].collider.GetComponent<Wheel>();
		  if (well == null)
			 continue;

		  if ((_phase != PhaseType.mult && well.IsMultiply) || (_phase != PhaseType.inc && !well.IsMultiply) || _phase == PhaseType.none)
			 continue;

		  bool isRound = false;

		  if (_phase == PhaseType.inc)
			 decrementValue = well.Value;
		  else
		  {
			 decrementValue *= well.Value;
			 isRound = true;
		  }

		  if (well.Rotate())
		  {
			 if (_phase == PhaseType.mult)
			 {
				Value -= decrementValue;
				decrementValue = 0;
				if (CheckResult())
				  return;
			 }

			 if(isRound)
				OnRoll?.Invoke();

			 PhaseType old = _phase;
			 _phase = PhaseType.none;

			 Move(old == PhaseType.mult ? _intFocus : _multiplyFocus, 0.5f, 1,()=> {
				_phase = old == PhaseType.mult ? PhaseType.inc : PhaseType.mult;
			 });
		  }
		  return;
		}
	 }

	 public override void StopInteract()
	 {

		DOTween.To(() => _text.transform.localScale, x => _text.transform.localScale = x, Vector3.zero, 1f);
		//DOTween.To(() => _light.intensity, x => _light.intensity = x, 0, 1f).OnComplete(()=> { _light.gameObject.SetActive(false); });
		//DOTween.To(() => _crystal.GetColor("_EmissionColor"), x => _crystal.SetColor("_EmissionColor", x), _deactiveCrystal, 1f);
		base.StopInteract();

		_light.DOIntensity(0, 1).OnComplete(() => { _light.gameObject.SetActive(false); }).SetDelay(0.5f);
		_crystal.DOColor(_deactiveCrystal, "_EmissionColor", 1);

		UnFocusCamera(0.5f);

		if (State != 2)
		{
		  State = 0;
		  ResetValue();
		  return;
		}
	 }
	 
	 private bool CheckResult()
	 {

		if (Value < 0)
		{
		  Failed();
		  StopInteract();
		  return true;
		}
		if (Value == 0)
		{
		  Complete();
		  return true;
		}
		return false;
	 }

	 void Move(Transform source, float duration, float wait, System.Action onComplete = null)
	 {
		float time = 0;
		Quaternion startRotate = _cameraFocus.localRotation;
		Vector3 startPosition = _cameraFocus.localPosition;
		_isMove = true;

		_cameraFocus.DOLocalMove(source.localPosition, duration).SetDelay(wait);
		_cameraFocus.DOLocalRotate(source.localRotation.eulerAngles, duration).SetDelay(wait).OnComplete(() =>
		{
		  _isMove = false;
		  onComplete?.Invoke();
		});

	 }

	 private void SetTest()
	 {
		_light.DOIntensity(7, 0.3f).OnComplete(() =>
		{
		  _light.DOIntensity(5, 0.3f);
		});
		_crystal.DOColor(_lightCrystal, "_EmissionColor", 0.3f).OnComplete(()=>{
		  _crystal.DOColor(_activeCrystal, "_EmissionColor", 0.3f);
		  _text.text = Value.ToString();
		});

	 }

	 private void ConfirmLightColor()
	 {
		_light.color = _lightCrystal;
	 }

	 public void ConfirmCrystalColor()
	 {
		_crystalRenderer.sharedMaterial = Instantiate(_crystalRenderer.sharedMaterial);
		_crystal = _crystalRenderer.sharedMaterial;
		_crystal.SetColor("_EmissionColor", _activeCrystal);
	 }

	 private void ConfirmTextCounter()
	 {
		_text.text = _maxValue.ToString();
	 }

	 private void ResetValue()
	 {
		Value = _maxValue;

		for(int i = 0; i < _whells.Length; i++)
		{
		  _whells[i].ResetData();
		}
	 }


	 [ContextMenu("Failed")]
	 protected override void Failed()
	 {
		base.Failed();
		StopInteract();
	 }

	 [ContextMenu("Complete")]
	 protected override void Complete()
	 {
		StopInteract();

		PegasusController pegasus = GetComponentInChildren<PegasusController>();
		if(pegasus != null)
		{
		  pegasus.Activate(() =>
		  {
			 base.Complete();
			 State = 2;

			 DOVirtual.DelayedCall(2f, () =>
			 {
				pegasus.Deactivate();
			 });

		  });
		}
		else
		  base.Complete();
		Save();
	 }

	 protected override void SetColliderActivate(bool isActive)
	 {
		for (int i = 0; i < _baseColliders.Length; i++)
		  _baseColliders[i].enabled = !isActive;
		for (int i = 0; i < _gameColliders.Length; i++)
		  _gameColliders[i].enabled = isActive;
	 }

  }
}