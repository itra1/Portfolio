using UnityEngine;

namespace UI
{
    public class LoaderPlayerAvatar : MonoBehaviour
    {
        [SerializeField] private it.UI.Avatar _avatar;
        [SerializeField] private bool _loadAvatarOnEnable = false;

        private void Awake()
        {
            _avatar.SetDefaultAvatar();
        }

        public void OnEnable()
        {
            if (_loadAvatarOnEnable)
            {
                _avatar.SetAvatar(UserController.User.AvatarUrl);
            }
        }
    }
}