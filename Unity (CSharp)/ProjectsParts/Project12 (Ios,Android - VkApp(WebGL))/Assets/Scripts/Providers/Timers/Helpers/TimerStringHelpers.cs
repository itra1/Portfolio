namespace Game.Scripts.Providers.Timers.Helpers
{
	class TimerStringHelpers
	{
		public static int Days(double seconds) => (int) (seconds / (60 * 60 * 24));
		public static int HoursTotal(double seconds) => (int) (seconds / (60 * 60));
		public static int Hours(double seconds) => (int) (seconds / (60 * 60)) % 24;
		public static int MinutTotal(double seconds) => (int) (seconds / 60);
		public static int Minut(double seconds) => (int) (seconds / 60) % 60;
		public static int Seconds(double seconds) => (int) seconds % 60;
	}
}
