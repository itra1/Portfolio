using Core.Network.Socket.Packets.Incoming.Actions.Consts;

namespace Elements.FloatingWindow.Presenter.Consts
{
	public static class ScreenPositionType
	{
		public const string LeftPosition = "left";
		public const string RightPosition = "right";
		public const string CenterPosition = "center";
		public const string AngleLeftPosition = "angle_left";
		public const string AngleRightPosition = "angle_right";
		
		public static string GetPosition(string alias)
		{
			return alias switch
			{
				FloatingWindowVideoStreamMaterialActionAlias.Left => LeftPosition,
				FloatingWindowVideoStreamMaterialActionAlias.Center => CenterPosition,
				FloatingWindowVideoStreamMaterialActionAlias.Right => RightPosition,
				FloatingWindowVideoStreamMaterialActionAlias.AngleLeft => AngleLeftPosition,
				FloatingWindowVideoStreamMaterialActionAlias.AngleRight => AngleRightPosition,
				_ => null
			};
		}
	}
}
