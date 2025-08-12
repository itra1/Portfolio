
public class GAnalytics : Singleton<GAnalytics> {

	private void Start() { }

	public void LogEvent(string space, string title, string desc, int param) {

#if PLUGIN_GOOGLE_ANALYTICS
		//GoogleAnalyticsV4.instance.LogEvent(space, title, desc, param);
#endif
	}

}
