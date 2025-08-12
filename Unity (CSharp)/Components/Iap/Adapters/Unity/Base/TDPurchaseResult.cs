namespace Platforms.Iap.Adapters.Unity.Base
{
	/// <summary>
	/// Результат покупки
	/// </summary>
	public class TDPurchaseResult
	{
		public bool IsSuccess { get; set; }
		public string ProductId { get; set; }
		public string StoreName { get; set; }
		public string StoreId { get; set; }
		public decimal LocalizedPrice { get; set; }
		public string IsoCurrencyCode { get; set; }
		public string Receipt { get; set; }
	}
}
