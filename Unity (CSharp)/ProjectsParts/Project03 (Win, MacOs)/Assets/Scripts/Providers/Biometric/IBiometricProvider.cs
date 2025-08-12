
using Cysharp.Threading.Tasks;

namespace Providers.Biometric
{
	public interface IBiometricProvider
	{
		bool CanAuthenticate { get; }
		bool SuccessAuth { get; }
		UniTask<bool> Authenticate();
		void SetAccess(bool isAccess);
	}
}
