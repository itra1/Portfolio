using System.Collections;
using UnityEngine.VFX;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  public class CoilElectricElement : MonoBehaviour, it.Game.Player.Interactions.IInteractionCondition
  {
	 public System.Action OnActivate;
	 public bool IsActivated { get; private set; }
	 private VisualEffect _sparks;
	 private Light _lights;

	 private void Awake()
	 {
		_sparks = GetComponentInChildren<VisualEffect>();
		_lights = GetComponentInChildren<Light>();
	 }

	 private void Start()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.InventaryGetItem, InventaryGetItem);
	 }

	 private void OnDestroy()
	 {
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.InventaryGetItem, InventaryGetItem);
	 }

	 private void InventaryGetItem(com.ootii.Messages.IMessage handler)
	 {
		string uuiditem = (string)handler.Data;

		if (uuiditem != "a929077e-3e35-4593-b33b-9ad2e2628865")
		  return;
		Activate();

	 }

	 /// <summary>
	 /// Активирует анимацию
	 /// </summary>
	[ContextMenu("Activate")]
	 public void Activate()
	 {
		StartCoroutine(UpdateCor());
	 }

	 public void Clear()
	 {
		IsActivated = false;
		StopAllCoroutines();
		_sparks.Stop();
		_lights.intensity = 0;
	 }

	 [ContextMenu("Interaction")]
	 public void Interaction()
	 {
		IsActivated = true;
		OnActivate?.Invoke();
		StopAllCoroutines();
		_sparks.Stop();
	 }

	 private IEnumerator UpdateCor()
	 {
		while (true)
		{
		  _sparks.Play();
		  _lights.DOIntensity(5f, 0.3f).OnComplete(() =>
		  {
			 _lights.DOIntensity(0, 0.7f);
		  });
		  yield return new WaitForSeconds(Random.Range(1f, 2f));
		}
	 }

	 public bool InteractionReady()
	 {
		return !IsActivated;
	 }
  }
}