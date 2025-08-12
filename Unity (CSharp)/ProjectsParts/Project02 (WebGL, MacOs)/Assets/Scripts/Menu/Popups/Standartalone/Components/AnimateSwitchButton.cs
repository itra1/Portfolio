
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
public class AnimateSwitchButton : MonoBehaviour,IPointerDownHandler
{
  [SerializeField]
  private RectTransform targetRect;

  public bool IsOn;
  public UnityAction<bool> OnValueChanged;
  private Vector2 startPosition;
  [SerializeField]
  private float animTime = 1F;
  private void Start()
  {
    startPosition = targetRect.anchoredPosition;
    Init();
  }

  private void Init(){
   if (IsOn){
      targetRect.DOAnchorPos(startPosition, animTime);
   }
   else{
      targetRect.DOAnchorPos(startPosition - new Vector2(40, 0), animTime);
   }
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    IsOn = !IsOn;
    Init();
  }
}
