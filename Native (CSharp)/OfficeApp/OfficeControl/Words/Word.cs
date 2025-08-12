/* ********************************************************
 * Контроллер по управлению Word
 * 
 * Документация https://learn.microsoft.com/ru-ru/dotnet/api/microsoft.office.interop.word
 * VBA: https://learn.microsoft.com/ru-ru/office/vba/api/overview/word
 * 
 * ********************************************************
 */

using System.Diagnostics;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using OfficeControl.Common;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Packages;

namespace OfficeControl.Words
{
	/// <summary>
	/// Документ Microsoft Word
	/// </summary>
	public partial class Word :OfficeApplication, IWord
	{
		/// <summary>
		/// Минимальное значение zoom
		/// </summary>
		public const int ZoomMin = 10;
		/// <summary>
		/// Максимальное значение zoom
		/// </summary>
		public const int ZoomMax = 500;
		/// <summary>
		/// Шаг зума
		/// </summary>
		public const int ZoomStep = 10;
		/// <summary>
		/// Шаг скрола
		/// </summary>
		public const int ScrollStep = 10;

		private readonly Application _app;
		private readonly Document _document;
		public override string AppType => OfficeAppType.Word;

		public Word(string filePath) : base(filePath)
		{
			Console.WriteLine("Try open Word app " + filePath);

			_app = new Microsoft.Office.Interop.Word.Application();
			_app.Visible = true;
			_document = _app.Documents.Open(filePath, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoFalse);
			_app.Caption = _uuid;

			try
			{
				if (!_document.CommandBars.GetPressedMso("MinimizeRibbon"))
				{
					_document.CommandBars.ExecuteMso("MinimizeRibbon");
				}

				_document.ActiveWindow.View.Type = WdViewType.wdPrintView;
				_app.DisplayStatusBar = false;
				_app.DisplayRecentFiles = false;
				_app.DisplayDocumentInformationPanel = false;
				_app.DisplayScreenTips = false;
				_app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
				_app.ShowWindowsInTaskbar = false;
				_app.ShowStartupDialog = false;

				_document.UserControl = false;
				_document.ActiveWindow.DisplayRulers = false;
				_document.ActiveWindow.DocumentMap = false;
				_document.ActiveWindow.WindowState = WdWindowState.wdWindowStateNormal;
				_document.ActiveWindow.Thumbnails = false;
				_document.CommandBars.DisplayTooltips = false;
				_document.CommandBars.AdaptiveMenus = false;
				_document.DisableFeatures = true;
				_document.ActiveWindow.View.Zoom.PageFit = WdPageFit.wdPageFitFullPage;
			}
			catch (System.Exception ex)
			{
				Console.WriteLine($"Ex {ex.Message} {ex.StackTrace}");
			}
		}

		public override void Close()
		{
			_app.ActiveWindow.Close(false);
			if (_app.Windows.Count <= 0)
			{
				_app.Quit();
				ClearEmptyProcess();
			}
		}

		private void ClearEmptyProcess()
		{
			var procList = Process.GetProcesses();
			for (var i = 0; i < procList.Length; i++)
			{
				if (procList[i].ProcessName == "WORD" && string.IsNullOrEmpty(procList[i].MainWindowTitle))
				{
					Console.WriteLine("Close ampty WORD process");
					procList[i].Kill();
					procList[i].Close();
					procList[i].Dispose();
					procList[i] = null;
				}
			}
		}

		public override Package MakeOpenCompletePackage()
		{
			return new CommonAppOpenComplete()
			{
				AppUuid = Uuid
			};
		}
	}
}
