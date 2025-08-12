
namespace it.Helpers
{
	public static class Currency
	{
		public static string Symbol => StringConstants.CURRENCY_SYMBOL;

		public static string String<T>(T value, bool visibleSymbol = true)
		{
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
			//return string.Format("{0}{1:###,###,###,###,###.##}", Symbol , value);

			string templateItem = "00";

			if (visibleSymbol)
				return string.Format(culture, "{0}{1:#,0." + templateItem + "}", Symbol, value);
			else
				return string.Format(culture, "{0:#,0." + templateItem + "}", value);
		}

		public static string String(int value, bool visibleSymbol = true)
		{
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
			//return string.Format("{0}{1:###,###,###,###,###.##}", Symbol , value);

			string templateItem = value % 1f != 0 ? "00" : "##";

			if (visibleSymbol)
				return string.Format(culture, "{0}{1:#,0." + templateItem + "}", Symbol, value);
			else
				return string.Format(culture, "{0:#,0." + templateItem + "}", value);
		}

		public static string CurrencyString(this int value, bool visibleSymbol = true){
			return String(value, visibleSymbol);
		}

		public static string String(float value, bool visibleSymbol = true)
		{
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
			//return string.Format("{0}{1:###,###,###,###,###.##}", Symbol , value);

			string templateItem = value%1f != 0 ? "00":"##";

			if (visibleSymbol)
				return string.Format(culture, "{0}{1:#,0."+ templateItem + "}", Symbol, value);
			else
				return string.Format(culture, "{0:#,0." + templateItem + "}", value);
		}

		public static string CurrencyString(this float value, bool visibleSymbol = true)
		{
			return String(value, visibleSymbol);
		}
		public static string String(decimal value, bool visibleSymbol = true)
		{
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
			//return string.Format("{0}{1:###,###,###,###,###.##}", Symbol , value);

			string templateItem = value%1m != 0 ? "00":"##";

			if (visibleSymbol)
				return string.Format(culture, "{0}{1:#,0."+ templateItem + "}", Symbol, value);
			else
				return string.Format(culture, "{0:#,0." + templateItem + "}", value);
		}

		public static string CurrencyString(this decimal value, bool visibleSymbol = true)
		{
			return String(value, visibleSymbol);
		}
		public static string String(double value, bool visibleSymbol = true)
		{
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
			//return string.Format("{0}{1:###,###,###,###,###.##}", Symbol , value);
			string templateItem = value % 1f != 0 ? "00" : "##";
			if (visibleSymbol)
				return string.Format(culture, "{0}{1:#,0." + templateItem + "}", Symbol, value);
			else
				return string.Format(culture, "{0:#,0." + templateItem + "}", value);
		}


		public static string CurrencyString(this double value, bool visibleSymbol = true)
		{
			return String(value, visibleSymbol);
		}


	}
}