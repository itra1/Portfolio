namespace Core.Materials.Loading.AutoPreloader
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает предварительную загрузку всех доступных материалов после авторизации
	/// </summary>
	public interface IAutoMaterialDataPreloader
	{
		bool IsLoadingCompleted { get; }
	}
}