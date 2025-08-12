using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Game.Elements.Weapons.Common;
using Game.Game.Elements.Weapons.Factorys;
using Game.Providers.Profile;
using ModestTree;

namespace Game.Game.Elements.Weapons
{
	public class WeaponSpawner : IWeaponSpawner
	{
		private IWeaponFactory _factory;
		private IProfileProvider _profileProvider;

		public bool IsLoaded { get; private set; }

		public WeaponSpawner(IWeaponFactory factory, IProfileProvider profileProvider)
		{
			_factory = factory;
			_profileProvider = profileProvider;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			if (_profileProvider.Weapons.IsEmpty())
			{
				_profileProvider.Weapons.Add(new()
				{
					IsUnlumited = true,
					Name = WeaponType.Knife
				});
			}

			await UniTask.Yield();
		}

		public IWeapon Spawn(string weaponType) => _factory.GetInstance(weaponType);

		public void AddWeapon(string name, int count)
		{
			var weaponProfile = _profileProvider.Weapons.Find(x => x.Name == name);

			if (weaponProfile == null)
			{
				weaponProfile = new() { Name = name, Count = count };
				_profileProvider.Weapons.Add(weaponProfile);
			}
			else
			{
				weaponProfile.Count += count;
			}
			_profileProvider.Save();
		}
	}
}
