namespace ExEvent {

	public class LanuageEvents {

		public sealed class OnChangeLanguage : BaseEvent {
			public LanuageTypesParam lang;

			public OnChangeLanguage(LanuageTypesParam lang) {
				this.lang = lang;
			}

			public static void Call(LanuageTypesParam lang) {
				BaseEvent.Call(new OnChangeLanguage(lang));
			}

		}

	}
}