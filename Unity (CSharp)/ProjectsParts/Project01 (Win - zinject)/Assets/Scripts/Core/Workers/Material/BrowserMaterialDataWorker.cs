using Core.Elements.Windows.Browser.Data;
using Core.Materials.Data;

namespace Core.Workers.Material
{
    public class BrowserMaterialDataWorker : WindowMaterialDataWorker
    {
        public override void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            base.PerformActionAfterAddingToStorageOf(material);
            
            if (material is not BrowserMaterialData browserMaterial)
                return;
            
            var authData = browserMaterial.AuthData;
            
            if (authData == null)
                return;
            
            var authScript = authData.AuthScript;
            
            if (string.IsNullOrEmpty(authScript)) 
                return;
            
            var username = authData.Username;
            var password = authData.Password;
            
            if (!string.IsNullOrEmpty(username))
                authScript = authScript.Replace("{{username}}", username);
            
            if (!string.IsNullOrEmpty(password))
                authScript = authScript.Replace("{{password}}", password);
            
            authData.AuthScript = authScript;
        }
    }
}