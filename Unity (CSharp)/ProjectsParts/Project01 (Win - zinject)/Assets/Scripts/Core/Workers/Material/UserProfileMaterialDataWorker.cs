using Core.Materials.Data;
using Core.User;
using Zenject;

namespace Core.Workers.Material
{
    public class UserProfileMaterialDataWorker : IAfterAddingToStorage
    {
        private IUserProfileSetter _userProfile;
        
        [Inject]
        private void Initialize(IUserProfileSetter userProfile) => _userProfile = userProfile;
        
        public void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            if (material is not UserProfileMaterialData userMaterial)
                return;

            _userProfile.Id = userMaterial.UserId;
            _userProfile.FirstName = userMaterial.FirstName;
            _userProfile.LastName = userMaterial.LastName;
            _userProfile.Email = userMaterial.Email;
        }
    }
}