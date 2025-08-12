namespace Game.Providers.Battles.Common
{
	partial class Battle
	{
		/// <summary>
		/// Очищаем оружие в руке
		/// </summary>
		protected virtual void RemoveSelectWeapons()
		{
			_playerGameHelper.RemoveWeapon();
		}
	}
}
