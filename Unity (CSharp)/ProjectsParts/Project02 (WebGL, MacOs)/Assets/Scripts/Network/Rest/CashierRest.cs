using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace it.Network.Rest
{
	public class Replenishment
	{
		public string provider;
		public decimal amount;
		public Requisites requisites;

		/// <summary>
		/// Необходим вывод QR
		/// </summary>
		public bool IsQR { get; set; }
	}

	public class Requisites
	{
		public string cardHolder;
		public string cardNumber;
		public string expireMonth;
		public string cardExpires;
		public string expireYear;
		public int cvv;
		public string phone;
		public string walletNumber;
		public string currency;
		public string currencyOut;
	}

	public class CashierMethods
	{
		public List<PayoutMethods> payout;
		public List<ReplaymenshipMethod> replenishment;
	}

	public interface ICashierMethod
	{
		public ulong Id { get; }
		public string Name { get; }
		public string Slug { get; }
		public int Order { get; }
		public decimal? MinLimit { get; }
		public decimal? MaxLimit { get; }
		public List<string> Icons { get; }
		public CashierMethosServiceInfo[] ServiceInfo { get; }
}

	public class PayoutMethods : ICashierMethod
	{
		public ulong id;
		public string name;
		public string slug;
		public int order;
		public decimal? min_limit;
		public decimal? max_limit;
		public List<string> icons;
		public CashierMethosServiceInfo[] service_info;

		public ulong Id => id;
		public string Name => name;
		public string Slug => slug;
		public int Order => order;
		public decimal? MinLimit => min_limit;
		public decimal? MaxLimit => max_limit;
		public List<string> Icons => icons;
		public CashierMethosServiceInfo[] ServiceInfo => service_info;
	}

	public class ReplaymenshipMethod : ICashierMethod
	{
		public ulong id;
		public string name;
		public string slug;
		public int order;
		public decimal? min_limit;
		public decimal? max_limit;
		public List<string> icons;
		public CashierMethosServiceInfo[] service_info;

		public ulong Id => id;
		public string Name => name;
		public string Slug => slug;
		public int Order => order;
		public decimal? MinLimit => min_limit;
		public decimal? MaxLimit => max_limit;
		public List<string> Icons => icons;
		public CashierMethosServiceInfo[] ServiceInfo => service_info;
	}

	public class CashierMethosServiceInfo{
		public string currency;
		public string icon;
	}

}