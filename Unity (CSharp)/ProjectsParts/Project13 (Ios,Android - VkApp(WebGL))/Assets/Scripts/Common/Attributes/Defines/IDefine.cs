namespace Game.Common.Attributes.Defines {
	public interface IDefine {
		string Description { get; }

		void AfterEnable();
		void AfterDisable();
	}
}
