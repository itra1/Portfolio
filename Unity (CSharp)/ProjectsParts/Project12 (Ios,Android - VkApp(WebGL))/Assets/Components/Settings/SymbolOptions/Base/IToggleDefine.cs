namespace Settings.SymbolOptions.Base {
	public interface IToggleDefine : IDefine {
		string Symbol { get; }

		void AfterEnable();
		void AfterDisable();
	}
}