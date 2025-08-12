using UnityEngine.Networking;

namespace Core.Network.Http.Utils
{
    public class ForceAcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}