using System.Text;

namespace Game.Providers.Timers.Helpers {
	public static class DoubleTimeHelper {

		public static string AsRealTimeString(this double value) {
			StringBuilder sb = new();
			var day = (int)(value / (60 * 60 * 24));
			var daySeconds = day * (60 * 60 * 24);
			if (day > 0)
				sb.Append(string.Format("{0:d2}:", day));
			var hour = (int)((value - daySeconds) / (60 * 60));
			var hourSeconds = (hour * (60 * 60));
			var minut = (int)((value - daySeconds - hourSeconds) / (60));
			var minutSeconds = (minut * 60);
			sb.Append(string.Format("{0:d2}:", hour));
			sb.Append(string.Format("{0:d2}", minut));
			if (day <= 0) {
				var seconds = (int)(value - daySeconds - hourSeconds - minutSeconds);
				sb.Append(string.Format(":{0:d2}", seconds));
			}
			return sb.ToString();
		}
		public static string AsRealTimeShortString(this double value) {
			StringBuilder sb = new();
			var day = (int)(value / (60 * 60 * 24));
			var daySeconds = day * (60 * 60 * 24);
			//if (day > 0)
			//	sb.Append(string.Format("{0:d2}:", day));
			var hour = (int)((value - daySeconds) / (60 * 60));
			var hourSeconds = (hour * (60 * 60));
			var minut = (int)((value - daySeconds - hourSeconds) / (60));
			var minutSeconds = (minut * 60);
			//sb.Append(string.Format("{0:d2}:", hour));
			sb.Append(string.Format("{0:d2}", minut));
			if (day <= 0) {
				var seconds = (int)(value - daySeconds - hourSeconds - minutSeconds);
				sb.Append(string.Format(":{0:d2}", seconds));
			}
			return sb.ToString();
		}
	}
}
