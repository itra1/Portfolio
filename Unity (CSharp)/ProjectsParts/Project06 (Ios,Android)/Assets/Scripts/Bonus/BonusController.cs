using UnityEngine;
using System.Collections;
using System;

public enum BonusTypes { live, energy };

/// <summary>
/// Контроллер бонуса
/// </summary>
public class BonusController : MonoBehaviour, IPlayerDamage {

    bool isFree;
    Vector3 velocity;
    [SerializeField] float gravity;
    [SerializeField] float groundLevel;
    public BonusTypes bonusType;
    public float  bonusValue;

    // Use this for initialization
    void Start () {
        isFree = true;
        InitDurable();
    }

    // Update is called once per frame
    void Update () {
        Muve();
    }

    public void YouMuved()
    {
        if (destroyThis != null) destroyThis(gameObject);
    }

    void Muve() {
        if(!isFree) return;

        velocity.y -= gravity * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;

        if(transform.position.y <= groundLevel) {
            isFree = false;
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
        }
    }

    #region Прочность
    [SerializeField] float durableMax;
    float durable;                                      // Запас прочности

	public bool DamageReady { get { return true; } }

	void InitDurable()
    {
        durable = durableMax;
    }

    public delegate void DestroyThis(GameObject target);
    public event DestroyThis destroyThis;

    //public void DamageEnemy(float damage)
    //{
    //    durable -= damage;
    //    if (durable <= 0)
    //    {
            
    //        Destroy(gameObject);
    //    }
    //}

    void OnDestroy()
    {
        if (destroyThis != null) destroyThis(gameObject);
    }

	public void Damage(float value) {
		durable -= value;
		if (durable <= 0) {

			Destroy(gameObject);
		}
	}
	#endregion
}
