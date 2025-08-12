using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Materials.Data
{
    /// <summary>
    /// Устаревшее название - "UserMaterial"
    /// </summary>
    [MaterialDataLoader("/users")]
    public class UserProfileMaterialData : MaterialData
    {
        [MaterialDataPropertyParse("id"), MaterialDataPropertyUpdate]
        public int UserId { get; set; }
        
        [MaterialDataPropertyParse("firstname"), MaterialDataPropertyUpdate]
        public string FirstName { get; set; }
        
        [MaterialDataPropertyParse("lastname"), MaterialDataPropertyUpdate]
        public string LastName { get; set; }
        
        [MaterialDataPropertyParse("email"), MaterialDataPropertyUpdate]
        public string Email { get; set; }
        
        public UserProfileMaterialData() => Model = MaterialModel.UserProfile;
    }
}