using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level2
{
  [System.Serializable]
  public class ScrollPuzzleItemEvent : UnityEngine.Events.UnityEvent<ScrollPuzzleItem>
  {
  }

  public class ScrollPuzzleItem : UUIDBase
  {
    [SerializeField]
    private GameObject _scroll;

    [HideInInspector]
    public ScrollPuzzleItemEvent onOpen;

    private bool _isOpen;
    public bool IsOpen { get => _isOpen; private set => _isOpen = value; }

    private void Awake()
    {
      if (onOpen == null)
        onOpen = new ScrollPuzzleItemEvent();
    }

    private void Start()
    {
      SetClose();
    }

    [ContextMenu("Set close")]
    public void SetClose()
    {
      IsOpen = false;
      _scroll.SetActive(false);
    }
    [ContextMenu("Set open")]
    public void SetOpen()
    {
      _scroll.SetActive(true);
      IsOpen = true;
    }

    [ContextMenu("Interaction Open")]
    public void InteractionOpen()
    {
      onOpen?.Invoke(this);
      SetOpen();
    }
  }
}