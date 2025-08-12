using Core.Elements.Windows.Camera.Data;
using Settings;
using UI.Audio.Controller;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public class VlcPlayerFactory : IVlcPlayerFactory
    {
        private readonly IPrefabSettings _prefabs;
        private readonly IVlcLibraryFactory _libraryFactory;
        private readonly IVlcMediaPlayerFactory _mediaPlayerFactory;
        private readonly IVlcMediaFactory _mediaFactory;
        private readonly IVlcPlayerTexturesFactory _texturesFactory;
        private readonly IAudioController _audio;
        
        public VlcPlayerFactory(IPrefabSettings prefabs,
            IVlcLibraryFactory libraryFactory, 
            IVlcMediaPlayerFactory mediaPlayerFactory, 
            IVlcMediaFactory mediaFactory,
            IVlcPlayerTexturesFactory texturesFactory,
            IAudioController audio)
        {
            _prefabs = prefabs;
            _libraryFactory = libraryFactory;
            _mediaPlayerFactory = mediaPlayerFactory;
            _mediaFactory = mediaFactory;
            _texturesFactory = texturesFactory;
            _audio = audio;
        }
        
        public IVlcPlayer CreatePlayer(RectTransform parent, CameraMaterialData material, string url) =>
            Create<VlcPlayer>(parent, material, url);
        
        public IVlcStream CreateStream(RectTransform parent, CameraMaterialData material, string url) =>
            Create<VlcStream>(parent, material, url);
        
        public IVlcStreamPlayer CreateStreamPlayer(IVlcStream stream, RectTransform parent)
        {
            if (stream == null)
            {
                Debug.LogError("An attempt was detected to initialize a VlcStreamPlayer instance with a null VlcStream reference");
                return null;
            }
            
            if (!TryBuild<VlcStreamPlayer>(parent, stream.Material, out var streamPlayer)) 
                return null;
            
            streamPlayer.Initialize(stream);
            
            return streamPlayer;
        }
        
        private TVlcPlayer Create<TVlcPlayer>(RectTransform parent, CameraMaterialData material, string url)
            where TVlcPlayer : VlcPlayer
        {
            if (material == null)
            {
                Debug.LogError($"An attempt was detected to initialize {typeof(TVlcPlayer).Name} with a null VideoMaterialData reference");
                return null;
            }
            
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError($"An attempt was detected to set a null or empty URL before {typeof(TVlcPlayer).Name} initializing");
                return null;
            }
            
            if (!TryBuild<TVlcPlayer>(parent, material, out var player)) 
                return null;
            
            player.SetUrl(url);
            player.Initialize(_libraryFactory, _mediaPlayerFactory, _mediaFactory, _texturesFactory, _audio);
            
            return player;
        }
        
        private bool TryBuild<TVlcPlayer>(RectTransform parent, CameraMaterialData material, out TVlcPlayer player) 
            where TVlcPlayer : VlcPlayerBase
        {
            if (!_prefabs.TryGetComponent<TVlcPlayer>(out var original))
            {
                Debug.LogError($"There is no prefab with the {typeof(TVlcPlayer).Name} component in the prefab settings");
                player = null;
                return false;
            }
            
            if (parent == null)
            {
                Debug.LogError($"An attempt was detected to assign a null parent to the {typeof(TVlcPlayer).Name}");
                player = null;
                return false;
            }
            
            var gameObject = Object.Instantiate(original.gameObject);
            
            gameObject.SetActive(false);
            
            if (!gameObject.TryGetComponent(out player))
                player = gameObject.AddComponent<TVlcPlayer>();
            
            player.SetName(player.GetType().Name);
            player.SetMaterial(material);
            player.SetParent(parent);
            player.AlignToParent();
            
            return true;
        } 
    }
}