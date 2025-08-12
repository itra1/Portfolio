namespace OfficeControl.Pipes.Base
{
	/// <summary>
	/// Базовый класс пакетов команд к office
	/// </summary>
	public abstract class Package
	{
		/// <summary>
		/// UUID экземпляра приложения
		/// </summary>
		public string AppUuid;

		/// <summary>
		/// UUID запроса
		/// </summary>
		public string RequestUuid;

		//public abstract string PackageType { get; }
	}
}
