using System.Collections;
using UnityEngine;

namespace Game.Map {

  public class MapPoint: MonoBehaviour
  {
    public System.Action<int> onClick;
    
    [SerializeField]
    private int _index;

    [SerializeField]
    private Animation animComponent;
    [SerializeField]
    private GameObject completeGraphic;
    [SerializeField]
    private GameObject activeGraphic;
    [SerializeField]
    private GameObject bossGraphic;
    [SerializeField]
    private GameObject nextGraphic;

    public bool isStartVisible = true;

    [SerializeField]
    private GameObject _connectObject;
    [SerializeField]
    private GameObject path;

    public void SetState(Location.StateType state)
    {
      if (path != null)
        path.SetActive(state == Location.StateType.complete);
      completeGraphic.SetActive(state == Location.StateType.complete);

      activeGraphic.SetActive(state == Location.StateType.active);
      bossGraphic.SetActive(state == Location.StateType.boss);
      nextGraphic.SetActive(state == Location.StateType.wait);

      if (_connectObject != null) {
        _connectObject.SetActive(state != Location.StateType.wait);
      }

      if (state == Location.StateType.active || state == Location.StateType.boss)
        StartCoroutine(PlayAnim());
    }

    private IEnumerator PlayAnim() {
      while (true) {
        animComponent.Play("play");
        yield return new WaitForSeconds(Random.Range(3, 7));
      }
    }

    public void OpenBattle() {
      onClick?.Invoke(_index);

    }

  }
  
}