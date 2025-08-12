using System.Collections.Generic;
using Game.Base.AppLaoder;
using Game.Providers.Smiles.Items;

namespace Game.Providers.Smiles
{
	public interface ISmilesProvider : IAppLoaderElement
	{
		List<SmileAsset> SmilesList { get; }
	}
}