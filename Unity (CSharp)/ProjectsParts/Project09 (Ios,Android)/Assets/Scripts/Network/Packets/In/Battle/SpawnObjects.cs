using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ObjectSpawn {
  public int id;
  public int typeNum;
  public ObjectType type;
  public object data;
  public Vector2 position;
  public Vector2 offset;
  public Vector2 size;
  public Vector3 velocity;
  
  public struct BulletData {
    public BulletType bulletType;
    public Vector2 targetPoint;
    public int parent;
    public float timeReload;
  }

  public struct BarrierData {
    public BarrierType barrierType;
    public float maxHealth;
    public float health;
  }

}

[System.Serializable]
public struct PlayerData {
  public string ownerId;
  public string name;
  public float maxHealth;
  public float health;
}

/// <summary>
/// Объекты генерируемые на сцене
/// </summary>
public class SpawnObjects : Packet {
  
  public List<ObjectSpawn> objectsList = new List<ObjectSpawn>();
  public int countObject;

  public override void ReadImpl() {
    objectsList.Clear();
    
    countObject = ReadH();
    
    for(int i = 0; i < countObject; i++) {
      ObjectSpawn os = new ObjectSpawn();

      os.id = ReadD();
      os.type = (ObjectType)ReadC();

      switch(os.type) {
        case ObjectType.Character:
          PlayerData player = new PlayerData();
          player.ownerId = ReadASCII();
          player.name = ReadASCII();
          player.maxHealth = ReadF();
          player.health = ReadF();
          os.data = player;
          break;
        case ObjectType.Bullet:
          ObjectSpawn.BulletData bullet = new ObjectSpawn.BulletData();
          bullet.bulletType = (BulletType)ReadC();
          bullet.targetPoint = ReadVec2();
          bullet.parent = ReadD();
          bullet.timeReload = ReadF();
          os.data = bullet;
          break;
        case ObjectType.Barrier:
          ObjectSpawn.BarrierData barrier = new ObjectSpawn.BarrierData();
          barrier.barrierType = (BarrierType)ReadC();
          barrier.maxHealth = ReadF();
          barrier.health = ReadF();
          os.data = barrier;
          break;
      }
      
      os.position = ReadVec2();
      os.offset = ReadVec2();
      os.size = ReadVec2();
      os.velocity = ReadVec2();
      
      objectsList.Add(os);
    }
  }

  public override void Process() {
    GameManager.instance.BattleAddSpawnObject(objectsList);
  }
  
}
