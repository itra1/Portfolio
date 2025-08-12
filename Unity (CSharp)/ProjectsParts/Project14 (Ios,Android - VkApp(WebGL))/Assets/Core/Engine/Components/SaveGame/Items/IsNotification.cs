namespace Core.Engine.Components.SaveGame
{
	/// <summary>
	/// Уведомления включены
	/// </summary>
	public class IsNotification : SaveProperty<bool>
	{
		public override bool DefaultValue => false;
	}
}
