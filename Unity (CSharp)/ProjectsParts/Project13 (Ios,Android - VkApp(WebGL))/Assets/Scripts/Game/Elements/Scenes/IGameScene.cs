using UnityEngine;
using UnityEngine.Splines;

namespace Game.Game.Elements.Scenes
{
	public interface IGameScene
	{
		Transform OpponentWeaponPoint { get; }
		Transform PlayerWeaponPoint { get; }
		Transform BoardPoint { get; }
		SplineContainer Spline { get; }
	}
}