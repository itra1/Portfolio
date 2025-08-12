using Microsoft.Office.Interop.PowerPoint;
using OfficeControl.Common;

namespace OfficeControl.PowerPoints
{
	public partial class PowerPoint :IPowerPointControl
	{
		public int SlideCurrentGet()
		{
			return _isPresentationMode
			? _slideShow.View.Slide.SlideIndex
			: _document != null ? _selectedIndex : 1;
		}
		public int SlideTotalGet()
		{
			return _isPresentationMode
			? _slideShow.Presentation.Slides.Count
			: 0;
		}
		public void SlideNextSet()
		{
			if (_document == null)
				return;

			if (_isPresentationMode)
			{
				if (_slideShow.View.Slide.SlideIndex == _pagesCount)
					return;

				_slideShow.View.Next();
			}
			else
			{
				SlideTargetSet(_selectedIndex + 1);
			}
		}
		public void SlidePreviousSet()
		{
			if (_document == null)
				return;

			if (_isPresentationMode)
			{
				_slideShow.View.Previous();
			}
			else
			{
				SlideTargetSet(_selectedIndex - 1);
			}
		}
		public void SlideTargetSet(int targetPage)
		{
			if (targetPage < 1)
				return;

			if (targetPage > _pagesCount)
				return;

			_slideShow.View.GotoSlide(targetPage);

		}
		public void PresentationModeToggle()
		{
			_isPresentationMode = !_isPresentationMode;
			Console.WriteLine($"Presentation mode {_isPresentationMode}");
			_document.SlideShowSettings.ShowType = PpSlideShowType.ppShowTypeWindow;

			if (_isPresentationMode)
			{
				_document.SlideShowSettings.AdvanceMode = PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
				_slideShow = _document.SlideShowSettings.Run();
				_slideShow.View.First();
			}
			else
			{
				_slideShow.View.Exit();
			}
		}
	}
}
