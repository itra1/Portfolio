using UnityEngine;
using System.Collections;

public class BarrelsController : MonoBehaviour {

    public float playerDmage;

    public GameObject sfxEffects;

    void OnTriggerEnter2D(Collider2D oth) {

        if(oth.GetComponent<Player.Jack.PlayerMove>()) {

            oth.GetComponent<Player.Jack.PlayerMove>().dragTime = Time.time + 0.5f;
            oth.GetComponent<Player.Jack.PlayerMove>().isDrag = true;

            if(sfxEffects != null) {
                GameObject sfx = Instantiate(sfxEffects, transform.position, Quaternion.identity) as GameObject;
                Destroy(sfx, 20);
            }
            /*
            oth.GetComponent<PlayerController>().ThisDamage(damagType.power, playerDmage, transform.position);

            if(sfxEffects != null) {
                GameObject sfx = Instantiate(sfxEffects, transform.position, Quaternion.identity) as GameObject;
                Destroy(sfx, 20);
            }
            */
        }
    }

}
