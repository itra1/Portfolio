namespace OfficeControl.Words
{
	public partial class Word
	{
		/// <summary>
		/// Отображение листа ниже
		/// </summary>
		public void ListDownMove()
		{
			_document.ActiveWindow.SmallScroll(Down: ScrollStep);
			Console.WriteLine("Word position " + _document.ActiveWindow.VerticalPercentScrolled);
		}

		/// <summary>
		/// Отображение листа выше
		/// </summary>
		public void ListUpMove()
		{
			_document.ActiveWindow.SmallScroll(Up: ScrollStep);
			Console.WriteLine("Word position " + _document.ActiveWindow.VerticalPercentScrolled);
		}

		/// <summary>
		/// Получить текущую позицию
		/// </summary>
		public int ListPositionGet()
		{
			Console.WriteLine("Word position return " + _document.ActiveWindow.VerticalPercentScrolled);
			return _document.ActiveWindow.VerticalPercentScrolled;
		}

		/// <summary>
		/// Установить позицию
		/// </summary>
		/// <param name="value"></param>
		public void ListPositionSet(int value)
		{
			_document.ActiveWindow.VerticalPercentScrolled = value;
		}

		/// <summary>
		/// Увеличение зума
		/// </summary>
		public void ZoomInSet()
		{
			int zoomValue = _document.ActiveWindow.View.Zoom.Percentage;
			_document.ActiveWindow.View.Zoom.Percentage = Math.Min(ZoomMax,zoomValue + ZoomStep);
		}

		/// <summary>
		/// Уменьшение зума
		/// </summary>
		public void ZoomOutSet()
		{
			int zoomValue = _document.ActiveWindow.View.Zoom.Percentage;
			_document.ActiveWindow.View.Zoom.Percentage = Math.Max(ZoomMin,zoomValue - ZoomStep);
		}

		/// <summary>
		/// Получение текущего зума
		/// </summary>
		/// <returns>Значение зума</returns>
		public int ZoomCurrentGet()
		{
			return _document.ActiveWindow.View.Zoom.Percentage;
		}
		/// <summary>
		/// Установка значения зума
		/// </summary>
		/// <returns>Значение зума</returns>
		public void ZoomCurrentSet(int value)
		{
			_document.ActiveWindow.View.Zoom.Percentage = value;
		}
	}
}
