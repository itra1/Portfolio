namespace Game.Scripts.Providers.Shop.Products
{
	public interface IProduct
	{
		string Type { get; }
		bool ReadyShow { get; }
	}
}