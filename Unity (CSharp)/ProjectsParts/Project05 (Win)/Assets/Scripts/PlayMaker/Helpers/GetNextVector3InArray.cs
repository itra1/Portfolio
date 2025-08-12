using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Helpers
{
  public class GetNextVector3InArray : FsmStateAction
  {

    [UIHint(UIHint.Variable)]
    [ArrayEditor(VariableType.GameObject)]
    public FsmArray _array;
    public FsmVector3 actualPosition;
    private int _indexPosition;

    public override void OnEnter()
    {
      base.OnEnter();
      _indexPosition++;
      if (_indexPosition >= _array.Values.Length)
        _indexPosition = 0;
      actualPosition.Value = ((GameObject)_array.Values[_indexPosition]).transform.position;
      Finish();
    }
  }
}