
namespace Providers.Network.Materials
{
	[System.Serializable]
	public class RegistrationData
	{
		public bool confirm;
		public string email;
		public string phone;
		public string password;
		public CurrencyData currency;
		public CountryData country;
		public string promo;
		public string password_confirmation;
		public string gc_ref;
	}
}
