using UnityEngine;

namespace it.Game.NPC.Enemyes
{
  /*
  Маскеруется ветошью.
  При приближении игрока начинает двигаться к нему и как добеется атакует.
  При потере игрока возвращается и занимает случайную позицию в радиусе 2х местов от стартовой точки

  */
  public class Mimic : Enemy
  {
    /*
     * Состояния:
     * 0 - ожидание
     * 1 - движение за игроком
     * 2 - атака
     * 3 - возвращашение на стартовую позицию
     * 
     */

    com.ootii.Actors.NavMeshDriver _navMeshDriver;
    private bool _isAttack = false;
    private float _lastAttackTime = 0;


    protected override void Start()
    {
      base.Start();
      StartPosition = transform.position;
      _navMeshDriver = GetComponent<com.ootii.Actors.NavMeshDriver>();
      //if(_navMeshDriver != null)
      //_navMeshDriver = null;
    }

    protected override void Update()
    {
      if (_navMeshDriver == null)
        return;

      CheckStates();
      AnimationConfirm();
    }

    void AnimationConfirm()
    {
      Animator.SetInteger("State", State);
      if(_isAttack)
      Animator.SetTrigger("Attack");
      _isAttack = false;
    }

    private void CheckStates()
    {
      if (State == 0)
      {
        if (PlayerCheck.IsPlayerVisible)
        {
          State = 1;
        }
        else
          return;
      }
      Vector3 playerPosition = it.Game.Player.PlayerBehaviour.Instance.transform.position;
      if (State == 1)
      {
        if ((playerPosition - _navMeshDriver.TargetPosition).magnitude > 0.2f)
          _navMeshDriver.TargetPosition = playerPosition;

        if ((playerPosition - transform.position).magnitude < 0.8f)
        {
          _navMeshDriver.ClearTarget();
          if(_lastAttackTime < Time.time + 3f)
          {
            _isAttack = true;
            _lastAttackTime = Time.time;
          }

        }
        if ((playerPosition - transform.position).magnitude > 20f)
        {
          _navMeshDriver.ClearTarget();
          State = 0;
        }
      }

      if (State == 2)
      {
        if ((playerPosition - transform.position).magnitude >= 1f)
        {
          State = 1;
        }
      }
    }


    [SerializeField]
    public Transform _target;

    [ContextMenu("Move")]
    public void SetTaget()
    {
      GetComponent<com.ootii.Actors.NavMeshDriver>().TargetPosition = _target.position;
    }

  }
}