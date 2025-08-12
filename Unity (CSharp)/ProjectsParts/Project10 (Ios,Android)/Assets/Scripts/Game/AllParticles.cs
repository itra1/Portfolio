using UnityEngine;
using System.Collections;

[System.Serializable]
public enum ParticleType {
    none,
    sandStoneParticle1,
    sandStoneParticle2
}

[System.Serializable]
public struct Particles {
    public GameObject prefab;
    public ParticleType type;
}

public class AllParticles : MonoBehaviour {

    public Particles[] particles;
    public static AllParticles backLink;

    void Start() {
        backLink = GetComponent<AllParticles>();
    }


    public static void Generate(ParticleType type, Vector3 pos, float time = 60) {
        foreach(Particles part in backLink.particles) {
            if (part.type == type) {
                GameObject inst = Instantiate(part.prefab, pos, Quaternion.identity) as GameObject;
                Destroy(inst, time);
            }
                
        }
    }

}
