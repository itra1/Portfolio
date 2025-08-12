
namespace ExEvent {
	public class LanguageEvents {

		public sealed class LanuageChange : BaseEvent {

			public static void Call() {
				BaseEvent.Call(new LanuageChange());
			}
		}

		public sealed class OnChangeLanguage : BaseEvent {

			public OnChangeLanguage() {
			}

			public static void Call() {
			}

		}

	}
}
