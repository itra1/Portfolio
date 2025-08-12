using UnityEngine;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  [System.Serializable]
  public class User : Player {

	 public int[] Buttons => _buttons;

	 private int[] _buttons = new int[2];

    protected override Vector3 StartChipPosition {
      get {
        return new Vector3 (1.13f, -1.07f, 0);
      }
    }

    protected override Color ChipColor {
      get {
        return Color.red;
      }
    }


	 public override void Step (System.Action start, System.Action moveComplete, System.Action<string, bool> onNextSection) {
      base.Step (start, moveComplete, onNextSection);
      RandomButton ();
    }

    private void RandomButton () {
		Buttons[0] = Random.Range(1, 5);
		Buttons[1] = Random.Range(1, 5);
      _manager.m_PiramideRight.SetValue (Buttons[0]);
      _manager.m_PiramideLeft.SetValue (Buttons[1]);
    }

    public void ButtonClick (int value) {
      _OnStart?.Invoke ();

      int step = value;
      Debug.Log ("User move " + step);

      if (CheckOutPath (step)) {
        LastMove = false;
        _OnCompleteMove?.Invoke ();
        return;
      }
      LastMove = true;

      _Chip.Move (GetPathForward (step), _OnCompleteMove, _OnMoveToNextSection);
    }
  }
}