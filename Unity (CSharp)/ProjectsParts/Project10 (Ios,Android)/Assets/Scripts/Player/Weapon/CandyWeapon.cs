using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;

public class CandyWeapon : MonoBehaviour {
    
    SkeletonRenderer skeletonRenderer;

    public float dragTime;                              // Время сдерживание
    public float speedX;
    Vector3 velocity;
    
    public WeaponTypes thisWeaponType;                // Текущий тип оружия

    List<int> listActive;
    bool act;
    bool shoot = false;

    void Start() {
        skeletonRenderer = Player.Jack.PlayerController.Instance.animation.skeletonRenderer;
        GetComponent<BoneFollower>().skeletonRenderer = skeletonRenderer;

        skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event += AnimEvent;
        
        act = true;
        listActive = new List<int>();
    }

    void OnDestroy() {
        skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event -= AnimEvent;
    }

    void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
        GetComponent<BoneFollower>().enabled = false;
        shoot = true;
    }

    void Update() {
        if(shoot) {
            velocity.x = RunnerController.Instance.runSpeedActual + speedX;
            transform.position += velocity * Time.deltaTime;
        }

        if(transform.position.x <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col) {

        if(!act) return;

        if(col.tag == "Enemy") {
            
            if(!listActive.Exists(x => x == col.gameObject.GetInstanceID())) {
                listActive.Add(col.gameObject.GetInstanceID());
                col.GetComponent<EnemyMove>().dragTime = Time.time + dragTime;
                shoot = false;
            }

            if(listActive.Count == 2) act = false;
        }
        
    }
}
