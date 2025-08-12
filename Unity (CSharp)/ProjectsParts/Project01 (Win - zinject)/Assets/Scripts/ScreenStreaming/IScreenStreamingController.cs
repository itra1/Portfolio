using Base.Presenter;

namespace ScreenStreaming
{
	public interface IScreenStreamingController
	{
		void Add(ulong areaMaterialId, IRenderStreamingCapable target);
		void Remove(ulong areaMaterialId);
	}
}