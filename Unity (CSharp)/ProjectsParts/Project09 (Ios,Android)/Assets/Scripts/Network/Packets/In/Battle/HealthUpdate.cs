using UnityEngine;
public class HealthUpdate : Packet {

  public static event Actione<HealthData> OnHealth;

  public struct HealthData {
    public int sceneId;
    public bool isDead;
    public float currentHealth;
    public float healthMax;
    public float damage;
    public float heal;
  }

  HealthData healthData = new HealthData();

  public override void ReadImpl() {
    healthData.sceneId = ReadD();
    healthData.isDead = ReadC() == 1 ? true : false;
    healthData.currentHealth = ReadF();
    healthData.healthMax = ReadF();
    healthData.damage = ReadF();
    healthData.heal = ReadF();
  }

  public override void Process() {
    if(OnHealth != null) OnHealth(healthData);
  }

}
