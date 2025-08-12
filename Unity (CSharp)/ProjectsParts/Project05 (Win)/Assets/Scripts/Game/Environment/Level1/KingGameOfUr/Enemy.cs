using UnityEngine;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  [System.Serializable]
  public class Enemy : Player {

    protected override Vector3 StartChipPosition {
      get {
        return new Vector3 (1.13f, 1.07f, 0);
      }
    }
    protected override Color ChipColor {
      get {
        return Color.black;
      }
    }

    public override void Step (System.Action start, System.Action moveComplete, System.Action<string, bool> onNextSection) {
      base.Step (start, moveComplete, onNextSection);
      int randomValue = Random.Range (1, 5);
      _OnStart?.Invoke ();

      if (CheckOutPath (randomValue)) {
        LastMove = false;
        _OnCompleteMove?.Invoke ();
        return;
      }
      LastMove = true;

      _Chip.Move (GetPathForward (randomValue), _OnCompleteMove, onNextSection);
    }

  }
}