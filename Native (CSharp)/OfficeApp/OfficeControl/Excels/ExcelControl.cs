using Microsoft.Office.Interop.Excel;

namespace OfficeControl.Excels
{
	/// <summary>
	/// Манипуляция с экселем
	/// </summary>
	public partial class Excel
	{

		/// <summary>
		/// Запрос текущей позиции строки
		/// </summary>
		/// <returns>Номер столбца</returns>
		public int PagePositionXGet()
		{
			return _app.ActiveWindow.ScrollColumn;
		}

		/// <summary>
		/// Запрос текущей позиции строки
		/// </summary>
		/// <returns>Номер строки</returns>
		public int PagePositionYGet()
		{
			return _app.ActiveWindow.ScrollRow;
		}

		/// <summary>
		/// Устанавливает целевую позицию
		/// </summary>
		/// <param name="x">Номер столбца</param>
		/// <param name="y">Номер страницы</param>
		public void PagePositionSet(int x,int y)
		{
			_app.ActiveWindow.ScrollColumn = x;
			_app.ActiveWindow.ScrollRow = y;
		}

		/// <summary>
		/// Отоюражение листа ниже
		/// </summary>
		public void PageDownMove()
		{
			_app.ActiveWindow.SmallScroll(Down: ScrollStep);
		}

		/// <summary>
		/// Отображение листа выше
		/// </summary>
		public void PageUpMove()
		{
			_app.ActiveWindow.SmallScroll(Up: ScrollStep);
		}

		/// <summary>
		/// Отображение листа левее
		/// </summary>
		public void PageLeftMove()
		{
			_app.ActiveWindow.SmallScroll(ToLeft: ScrollStep);
		}

		/// <summary>
		/// Отображение листа правее
		/// </summary>
		public void PageRightMove()
		{
			_app.ActiveWindow.SmallScroll(ToRight: ScrollStep);
		}

		/// <summary>
		/// Следующий лист
		/// </summary>
		public void PageNextSet()
		{
			int targetIndex = ((Worksheet)_app.ActiveWorkbook.ActiveSheet).Index;

			targetIndex++;
			if (targetIndex > _app.Sheets.Count)
			{
				targetIndex = 1;
			}
			((Worksheet)_app.Sheets[targetIndex]).Select(targetIndex);
		}

		/// <summary>
		/// предыдущий лист
		/// </summary>
		public void PagePrevSet()
		{
			int targetIndex = ((Worksheet)_app.ActiveWorkbook.ActiveSheet).Index;

			targetIndex--;
			if (targetIndex < 1)
			{
				targetIndex = _app.Sheets.Count;
			}
			((Worksheet)_app.Sheets[targetIndex]).Select(targetIndex);
		}

		/// <summary>
		/// Устанавливаем Целевую страницу
		/// </summary>
		/// <param name="index"></param>
		public void PageIndexSet(int index)
		{
			if (index < 1 || index > _app.Sheets.Count)
				return;

			((Worksheet)_app.Sheets[index]).Select(index);
		}

		/// <summary>
		/// Получем текущую страницу
		/// </summary>
		/// <returns>Индекс текущей страницы</returns>
		public int PageIndexGet()
		{
			return ((Worksheet)_app.ActiveWorkbook.ActiveSheet).Index;
		}

		/// <summary>
		/// Увеличение зума
		/// </summary>
		public void ZoomInSet()
		{
			double zoomValue = _app.ActiveWindow.Zoom;
			_app.ActiveWindow.Zoom = Math.Min(ZoomMax,zoomValue + ZoomStep);
		}

		/// <summary>
		/// Уменьшение зума
		/// </summary>
		public void ZoomOutSet()
		{
			double zoomValue = _app.ActiveWindow.Zoom;
			_app.ActiveWindow.Zoom = Math.Max(ZoomMin,zoomValue - ZoomStep);
		}
		public void ZoomCurrentSet(double value)
		{
			_app.ActiveWindow.Zoom = value;
		}

		/// <summary>
		/// Получение текущего зума
		/// </summary>
		/// <returns>Значение зума</returns>
		public double ZoomCurrentGet()
		{
			return _app.ActiveWindow.Zoom;
		}
	}
}
