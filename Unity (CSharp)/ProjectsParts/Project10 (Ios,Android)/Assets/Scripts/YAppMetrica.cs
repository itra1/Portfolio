public class YAppMetrica : Singleton<YAppMetrica> {

	private void Start() {}
	
	public void ReportEvent(string eventName) {

#if PLUGIN_YANDEXMETRICA
		AppMetrica.Instance.ReportEvent(eventName);
#endif
	}
}
