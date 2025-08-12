/* ������ �������� ����
 * 
 * ������������: https://api.telegram.org
 * ���: https://core.telegram.org/bots/api
 */

namespace Telegram
{
	public partial class TelergamClient
	{
		private string _botKey;

		private string Server => $"https://api.telegram.org/{_botKey}";

		public TelergamClient(string botKey)
		{
			_botKey = botKey;
		}
	}
}
