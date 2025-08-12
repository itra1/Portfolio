using UnityEngine;
using System.Collections;

public abstract class EnemyBoss : Enemy {

    public event deadMinion OnDeadMinion;
    public delegate void deadMinion();

    public delegate void BossDead();
    public BossDead OnBossDead;

    /// <summary>
    /// Вектор движения
    /// </summary>
    [HideInInspector]
    public Vector3 velocity;

    public override void OnEnable() {
        base.OnEnable();
        //MoveFunction = Move;
    }

    public override void OnDisable() {
        base.OnDisable();
    }

    public virtual void Move() { }

    public virtual void DeadMinion() {
        if (OnDeadMinion != null)
            OnDeadMinion();
    }

    protected void DeadBoss() {
        if (OnBossDead != null)
            OnBossDead();
        OnBossDead = null;
    }
}
