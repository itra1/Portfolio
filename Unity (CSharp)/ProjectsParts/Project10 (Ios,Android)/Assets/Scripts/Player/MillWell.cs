using UnityEngine;
using System.Collections;

public class MillWell : MonoBehaviour {

    public LayerMask coinsMask;
    void Update() {
        ChackCoins();
    }

    void OnTriggerEnter2D(Collider2D other) {

        // При контакте с монетой, добавляем
        if(other.tag == "Coins") {
            other.GetComponent<Coin>().AddCouns();
        }
    }

    void ChackCoins() {

        Ray ray = new Ray(transform.position, Vector3.left);

        RaycastHit[] allcoins = Physics.SphereCastAll(ray,1,7,coinsMask);

        foreach(RaycastHit oneHit in allcoins)
            oneHit.transform.GetComponent<Coin>().AddCouns();
    }
}
