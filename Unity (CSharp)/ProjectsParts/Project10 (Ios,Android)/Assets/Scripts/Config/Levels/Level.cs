using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Configuration.Levels {
  public class Level {

    public string name;
    public float distance;
    public List<Barriers> barriers;
    public List<Bonus> bonus;
    public List<Decoration> decoration;
    public List<Enemy> enemy;
    public List<Magic> magic;
    public List<Item> items;
    public List<Pet> pets;
    public List<Platform> platforms;
    public List<Quest> questions;
    public List<Road> road;
    public List<SpecialBarrier> specialBarrier;

  }
}