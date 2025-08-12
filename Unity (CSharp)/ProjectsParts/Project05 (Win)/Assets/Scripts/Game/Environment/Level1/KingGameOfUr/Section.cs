using UnityEngine;
namespace it.Game.Environment.Challenges.KingGameOfUr
{

  /// <summary>
  /// Серция на доске
  /// </summary>
  public class Section : UUIDBase
  {

	 [SerializeField]
	 public bool _super;

	 public bool Super => _super;


  }

}