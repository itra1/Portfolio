namespace Game.Providers.Ui.Windows {
	public interface IWindowProvider {
		Base.Presenter GetWindow(string name, bool isSplash = false);
		void CloseAllWindows();
	}
}
