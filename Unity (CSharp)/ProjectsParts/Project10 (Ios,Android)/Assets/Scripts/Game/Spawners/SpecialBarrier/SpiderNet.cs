/*
 Фиксатор паутины
*/

using UnityEngine;
using System.Collections;

public class SpiderNet : MonoBehaviour {

    public GameObject LeftFix;
    public GameObject RightFix;
    public GameObject LineNet;
    public GameObject decor1;
    public GameObject decor2;
    public GameObject decor3;

    public float platformWidth;
    public LayerMask platformMask;

    void Start() {
        Ray ray = new Ray(transform.position, Vector3.left);
        RaycastHit[] allcoins = Physics.SphereCastAll(ray, 3, 10, platformMask);
        
        if (allcoins.Length > 0) {
            Vector3 startBreack  = new Vector3(-100,0,0);
            foreach (RaycastHit one in allcoins)
            {
                if (one.transform.position.x > startBreack.x && one.transform.tag == "jumpUp") {
                    startBreack = one.transform.position;
                }
            }
            ActiveNet(startBreack);
        }
    }

    void ActiveNet(Vector3 startBreack)
    {
        float distance = transform.position.x - startBreack.x - platformWidth;
        transform.position = new Vector3(startBreack.x + ((transform.position.x - startBreack.x) / 2), transform.position.y + 0.1f, transform.position.z);
        LeftFix.transform.position = new Vector3(transform.position.x - distance / 2 - 0.1f, LeftFix.transform.position.y, LeftFix.transform.position.z);
        RightFix.transform.position = new Vector3(transform.position.x + distance / 2 + 0.1f, LeftFix.transform.position.y, LeftFix.transform.position.z);
        LineNet.transform.localScale = new Vector3(LineNet.transform.localScale.x * (distance / platformWidth), LineNet.transform.localScale.y, LineNet.transform.localScale.z);
        decor1.transform.position = new Vector3(Random.Range(transform.position.x - (distance - 0.4f) / 2, transform.position.x + (distance - 0.4f) / 2), decor1.transform.position.y, decor1.transform.position.z);
        decor2.transform.position = new Vector3(Random.Range(transform.position.x - (distance - 0.1f) / 2, transform.position.x + (distance - 0.1f) / 2), decor2.transform.position.y, decor2.transform.position.z);
        decor3.transform.position = new Vector3(Random.Range(transform.position.x - (distance - 0.1f) / 2, transform.position.x + (distance - 0.1f) / 2), decor3.transform.position.y, decor3.transform.position.z);
        LineNet.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(distance/ platformWidth*1.7f, 1);
        GetComponent<BoxCollider>().size = new Vector3(distance / platformWidth * 2.5f, 1, 1);
        GetComponent<BoxCollider>().center = new Vector3(0, -0.6f, 0);
    }

}
