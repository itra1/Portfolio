/* ********************************************************
 * Контроллер по управлению Excel
 * 
 * Документация https://learn.microsoft.com/ru-ru/dotnet/api/microsoft.office.interop.excel
 * VBA: https://learn.microsoft.com/ru-ru/office/vba/api/overview/excel
 * 
 * ********************************************************
 */

using System.Diagnostics;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using OfficeControl.Common;
using OfficeControl.Helpers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Packages;

namespace OfficeControl.Excels
{
	public partial class Excel :OfficeApplication, IExcel
	{
		/// <summary>
		/// Минимальное значение zoom
		/// </summary>
		public const double ZoomMin = 10;
		/// <summary>
		/// Максимальное значение zoom
		/// </summary>
		public const double ZoomMax = 400;
		/// <summary>
		/// Шаг зума
		/// </summary>
		public const double ZoomStep = 10;
		/// <summary>
		/// Шаг скрола
		/// </summary>
		public const int ScrollStep = 1;

		private readonly Application _app;
		private readonly Workbook _document;
		public override string AppType => OfficeAppType.Excel;

		public Excel(string filePath) : base(filePath)
		{
			Console.WriteLine("Try open Excel app " + filePath);

			_app = new Application();
			_app.Visible = true;
			_document = _app.Workbooks.Open(filePath, MsoTriState.msoFalse, MsoTriState.msoTrue, IgnoreReadOnlyRecommended: MsoTriState.msoTrue);
			_app.Caption = _uuid;
			try
			{
				_app.ExecuteExcel4Macro("SHOW.TOOLBAR(\"Ribbon\", false)");
				//_document.CommandBars.ExecuteMso("MinimizeRibbon");
				//_app.DisplayStatusBar = false;
				//_app.DisplayRecentFiles = false;
				//_app.DisplayDocumentInformationPanel = false;
				//_app.DisplayAlerts = false;
				//_app.ShowWindowsInTaskbar = false;
				//_app.ShowStartupDialog = false;

				//_document.UserControl = false;
				//_document.CommandBars.DisplayTooltips = false;
				//_document.CommandBars.AdaptiveMenus = false;
			}
			catch (System.Exception ex)
			{
				Console.WriteLine($"Ex {ex.Message} {ex.StackTrace}");
			}
			ProcessApp();

			//try
			//{
			//	for (int i = 0; i < _app.ActiveWindow.Panes.Count; i++)
			//	{
			//		var elem = _app.ActiveWindow.Panes[i];
			//		Console.WriteLine($"Panel {elem.Index} : parent");
			//	}
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine($"Error panelList {ex.Message} : parent {ex.StackTrace}");
			//}
		}

		private void ProcessApp()
		{
			var procList = Process.GetProcesses();
			for (var i = 0; i < procList.Length; i++)
			{
				if (procList[i].ProcessName == "EXCEL")
				{
					Console.WriteLine(procList[i].ProcessName + " : " + procList[i].MainWindowTitle);
					var window = procList[i].MainWindowHandle;
					_ = WindowApi.SetWindowPos(window, (IntPtr)WindowApi.SpecialWindowHandles.HWND_BOTTOM, 0, 0, 0, 0, WindowApi.SetWindowPosFlags.SWP_NOMOVE | WindowApi.SetWindowPosFlags.SWP_NOSIZE);
				}
			}
		}

		public override void Close()
		{
			_ = _app.ActiveWindow.Close(false);
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
				if (procList[i].ProcessName == "EXCEL" && string.IsNullOrEmpty(procList[i].MainWindowTitle))
				{
					Console.WriteLine("Close ampty EXCEL process");
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
