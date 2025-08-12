namespace Game.Scripts.Providers.Shop.Prices
{
	public interface IPrice
	{
		float Value { get; }
		float OldValue { get; }
	}
}