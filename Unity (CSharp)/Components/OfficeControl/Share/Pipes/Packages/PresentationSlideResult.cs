using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	/// <summary>
	/// Ответ на запрос номера слайда
	/// </summary>
	[PackageName(PackagesNames.PresentationPageResult)]
	public partial class PresentationSlideResult :PresentationPackage
	{
		public int Slide;
		public int TotalSlide;

		public PresentationSlideResult(int slideIndex, int totalSlide)
		{
			Slide = slideIndex;
			TotalSlide = totalSlide;
		}
	}
}
