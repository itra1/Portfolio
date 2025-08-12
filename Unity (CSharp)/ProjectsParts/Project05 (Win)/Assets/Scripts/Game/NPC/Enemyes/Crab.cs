using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace it.Game.NPC.Enemyes
{
  public class Crab : Enemy
  {
    protected virtual bool shouldRecalculatePath
    {
      get
      {
        return Time.time - lastRepath >= repathRate && !waitingForPathCalculation && canSearch && !float.IsPositiveInfinity(destination.x);
      }
    }
    public Vector3 destination { get; set; }
    protected float lastRepath = float.NegativeInfinity;
    public float repathRate = 0.5f;
    protected bool waitingForPathCalculation = false;
    [UnityEngine.Serialization.FormerlySerializedAs("repeatedlySearchPaths")]
    public bool canSearch = true;
    [System.NonSerialized]
    public bool updatePosition = true;
    public Vector3 position { get { return updatePosition ? transform.position : simulatedPosition; } }
    protected Vector3 simulatedPosition;

    [SerializeField]
    private Transform _transformTarget;

    protected override void Start()
    {
      base.Start();
    }

    protected override void PhaseChange()
    {
      base.PhaseChange();

      if((Phase & NpcPhase.free) != 0)
      {

      }

    }

    [ContextMenu("Attack")]
    public void Attack()
    {
      var fsms = GetComponents <PlayMakerFSM>();
      for (int i =0; i < fsms.Length ; i++)
      {
        if (fsms[i].Fsm.Name == "Behaviour")
        {
          fsms[i].FsmVariables.GetFsmGameObject("Player").Value = it.Game.Player.PlayerBehaviour.Instance.gameObject;
          fsms[i].Fsm.Event("Attack");
        }
      }
    }

	 protected override void Update()
	 {
		base.Update();
      //if (shouldRecalculatePath) SearchPath();
    }

	 //public virtual void SearchPath()
  //  {
  //    Debug.Log("SearchPath");
  //    if (float.IsPositiveInfinity(destination.x)) return;

  //    lastRepath = Time.time;
  //    //waitingForPathCalculation = true;

  //    seeker.CancelCurrentPathRequest();

  //    Vector3 start, end;
  //    CalculatePathRequestEndpoints(out start, out end);

  //    // Alternative way of requesting the path
  //    //ABPath p = ABPath.Construct(start, end, null);
  //    //seeker.StartPath(p);

  //    // This is where we should search to
  //    // Request a path to be calculated from our current position to the destination
  //    Debug.Log(end);
  //    seeker.StartPath(start, end);
  //  }

    protected virtual void CalculatePathRequestEndpoints(out Vector3 start, out Vector3 end)
    {
      start = GetFeetPosition();
      //end = destination;
      end = _transformTarget.position;
    }

    public virtual Vector3 GetFeetPosition()
    {
      return position;
    }

  }
}