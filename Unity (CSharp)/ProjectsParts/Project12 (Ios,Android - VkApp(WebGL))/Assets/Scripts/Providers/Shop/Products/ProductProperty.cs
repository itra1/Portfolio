using Game.Scripts.Providers.Shop.Settings;

namespace Game.Scripts.Providers.Shop.Products
{
	public abstract class ProductProperty<TProperty> : Product, IProductProperty<TProperty>
	where TProperty : ProductPropertyBase
	{
		protected TProperty _property;

		public TProperty Prooperty => _property;

		public override void SetProductProperty(ProductPropertyBase productProperty)
		{
			_property = (TProperty) productProperty;
		}
	}
}
