using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level1
{
  public class DeersGroup : Environment
  {
    public GameObject[] _prefabs;

    protected override void Start()
    {
      base.Start();
      for(int i = 0; i < _prefabs.Length; i++)
      {
        _prefabs[i].SetActive(false);
      }
    }

    protected override void ConfirmState(bool isForce = false)
    {
      base.ConfirmState(isForce);

      if(State == 0)
      {
        ClearExists();
        Instances();
      }
      else
      {
        ClearExists();
      }

    }

    private void Instances()
    {
      for (int i = 0; i < _prefabs.Length; i++)
      {
        GameObject inst = Instantiate(_prefabs[i], _prefabs[i].transform.parent);
        inst.transform.position = _prefabs[i].transform.position;
        inst.SetActive(true);

        //PlayMakerFSM[] fsms = inst.GetComponents<PlayMakerFSM>();
        //for (int x = 0; x < fsms.Length; x++)
        //{
        //  var player = fsms[x].FsmVariables.GetFsmGameObject("Player");
        //  if (player != null)
        //  {
        //    player.Value = it.Game.Player.PlayerBehaviour.Instance.gameObject;
        //  }
        //}
      }
    }

    private void ClearExists()
    {
      it.Game.NPC.Animals.Deer[] deers = GetComponentsInChildren<it.Game.NPC.Animals.Deer>();

      for (int i = 0; i < deers.Length; i++)
      {
        if(deers[i].gameObject.activeInHierarchy)
          DestroyImmediate(deers[i].gameObject);
      }
    }

    public void PlayerHere()
    {
      if (State == 1)
        return;

      State = 1;
      Save();
    }

  }
}