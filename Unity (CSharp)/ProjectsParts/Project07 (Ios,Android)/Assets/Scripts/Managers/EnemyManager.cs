using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager: Singleton<EnemyManager> {
  [SerializeField]
  private List<Sprite> _sprites;

  private int _actualBlock = -1;
  public List<Formation> formations;
  [SerializeField]
  private Position bossPosition;
  public Position BossPosition {
    get { return bossPosition; }
  }

  public Formation GetRandomFormation() {
    return GetFormation(Random.Range(0, formations.Count));
  }
  public Formation GetFormation(int number) {
    return formations[Random.Range(0, formations.Count)];
  }

  public void LoadEnemyGraphic(int block) {

    if (_actualBlock == block)
      return;

    _sprites.Clear();
    Resources.UnloadUnusedAssets();
    _sprites = Resources.LoadAll<Sprite>("Graphic/Enemy/" + block).ToList();

  }

  public int GetEnemyGraphicCount()
  {
    return _sprites.Count;
  }

  public Sprite GetSprite(int index)
  {
    return _sprites[index];
  }

#if UNITY_EDITOR

  [ContextMenu("Find enemy")]
  private void FindEnemy() {

    Enemy[] enemArr = GameObject.FindObjectsOfType<Enemy>();

    Formation format = new Formation();

    for (int i = 0; i < enemArr.Length; i++) {

      Position enemPos = new Position();
      enemPos.position = enemArr[i].transform.position;
      enemPos.scaling = enemArr[i].transform.localScale;
      format.enemyPositions.Add(enemPos);
    }
    formations.Add(format);

  }

#endif

  [System.Serializable]
  public class Formation {

    public List<Position> enemyPositions;

    public Formation() {
      enemyPositions = new List<Position>();
    }

  }

  [System.Serializable]
  public class Position {
    public Vector3 position;
    public Vector3 scaling;
  }

}
