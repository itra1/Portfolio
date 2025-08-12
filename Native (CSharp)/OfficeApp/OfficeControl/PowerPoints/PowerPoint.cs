/* ********************************************************
 * Контроллер по управлению PowerPoint
 * 
 * Документация https://learn.microsoft.com/ru-ru/dotnet/api/microsoft.office.interop.powerpoint
 * VBA: https://learn.microsoft.com/ru-ru/office/vba/api/overview/powerpoint
 * 
 * ********************************************************
 */

using System.Diagnostics;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using OfficeControl.Common;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Packages;

namespace OfficeControl.PowerPoints
{
	public partial class PowerPoint :OfficeApplication, IPowerPoint
	{
		private const string ProcessName = "POWERPNT";
		private readonly Microsoft.Office.Interop.PowerPoint.Application _app;
		private readonly Presentation _document;
		private SlideShowWindow _slideShow;
		private bool _isPresentationMode;
		private int _pagesCount;
		private int _selectedIndex;

		public override string AppType => OfficeAppType.PowerPoint;

		public Microsoft.Office.Interop.PowerPoint.Application App { get => _app; }

		public PowerPoint(string filePath) : base(filePath)
		{
			Console.WriteLine("Try open Powerpoint app " + filePath);

			_app = new Microsoft.Office.Interop.PowerPoint.Application();
			_document = _app.Presentations.Open(FilePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);

			_selectedIndex = 1;
			_pagesCount = _document.Slides.Count;

			_app.SlideSelectionChanged += (handle) =>
			{
				_selectedIndex = handle.SlideIndex;
			};

			PresentationModeToggle();
			_slideShow.Application.Caption = _uuid;
		}

		~PowerPoint()
		{
			System.Console.WriteLine("PowerPoint destructor");
			Close();
		}

		public override Package MakeOpenCompletePackage()
		{
			return new CommonAppOpenComplete()
			{
				AppUuid = Uuid
			};
		}

		public override void Close()
		{
			System.Console.WriteLine("PowerPoint Close");
			if (_isPresentationMode)
			{
				_slideShow.View.Exit();
				if (_app != null)
					_app.Quit();
			}
			else
			{
				_document.Close();
			}

			try
			{
				if (_app == null || _app.Presentations == null || _app.Presentations.Count <= 0)
				{
					ClearEmptyProcess();
				}
			}
			catch (Exception)
			{
				ClearEmptyProcess();
			}

		}

		private void ClearEmptyProcess()
		{
			try
			{
				_app.Quit();
			}
			catch (Exception)
			{
			}
			var procList = Process.GetProcesses();
			for (var i = 0; i < procList.Length; i++)
			{
				if (procList[i].ProcessName == "POWERPNT" && ((string.IsNullOrEmpty(procList[i].MainWindowTitle) || procList[i].MainWindowTitle == "PowerPoint")))
				{
					Console.WriteLine(procList[i].ProcessName);
					Console.WriteLine("Close ampty POWERPOINT process");
					procList[i].Kill();
					procList[i].Close();
					procList[i].Dispose();
					procList[i] = null;
				}
			}
		}
	}
}
