
namespace OfficeControl.Common
{
	public interface IPowerPointControl
	{
		int SlideCurrentGet();
		void SlideNextSet();
		void SlidePreviousSet();
		void SlideTargetSet(int targetPage);
		void PresentationModeToggle();
	}
}
