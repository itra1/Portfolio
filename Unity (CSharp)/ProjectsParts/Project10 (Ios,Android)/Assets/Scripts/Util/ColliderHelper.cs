using UnityEngine;

/// <summary>
/// Для определения столкновения на коллайдерах
/// </summary>
public class ColliderHelper : MonoBehaviour {

    public delegate void ColliderContact(Collider2D contact);
    public event ColliderContact OnColliderContact;

    void OnTriggerEnter2D(Collider2D coll) {
        if (OnColliderContact != null) OnColliderContact(coll);
    }


}
