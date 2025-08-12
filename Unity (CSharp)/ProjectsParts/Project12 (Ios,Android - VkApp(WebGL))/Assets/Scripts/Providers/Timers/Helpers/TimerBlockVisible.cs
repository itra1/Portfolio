using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Providers.Timers.Ui;

namespace Game.Scripts.Providers.Timers.Helpers
{
	public class TimerBlockVisible
	{
		public static void VisibleBlock(TimerPresenterBlock block, double seconds)
		{
			Dictionary<string, int> times = new(){
				{StringConstant.DaysLabel,TimerStringHelpers.Days(seconds) },
				{StringConstant.HoursLabel,TimerStringHelpers.Hours(seconds) },
				{StringConstant.MinutesLabel,TimerStringHelpers.Minut(seconds) },
				{StringConstant.SecondsLabel,TimerStringHelpers.Seconds(seconds) }
			};

			int timeIndex = 0;
			bool existsOlder = false;

			for (int i = 0; i < block.Items.Length; i++)
			{
				int value = 0;
				string label = StringConstant.SecondsLabel;

				while (timeIndex < times.Count && (timeIndex < (times.Count - (block.Items.Length - i - 1))
				|| value == 0)
				)
				{
					value = times.ElementAt(timeIndex).Value;
					label = times.ElementAt(timeIndex).Key;
					timeIndex++;
					if (value > 0)
						break;

					if (existsOlder)
						break;
				}
				existsOlder = true;
				block.Items[i].ValueLabel.text = value.ToString("00");
				block.Items[i].TitleLabel.text = label.ToLower();
			}
		}
	}
}
