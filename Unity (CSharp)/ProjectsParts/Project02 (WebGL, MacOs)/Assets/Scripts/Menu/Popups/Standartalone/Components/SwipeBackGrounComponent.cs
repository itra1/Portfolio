using UnityEngine;
[RequireComponent(typeof(ToggleImageSwipe))]
[AddComponentMenu("Custom/ToggleImagesSwipeActionComponent")]
public sealed class SwipeBackGrounComponent : SwiperImageBase
{
  [SerializeField]
  private GameObject background;
  public override void Swipe(bool value)
  {
    background?.SetActive(value);
  }
}

