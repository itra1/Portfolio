#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public interface IFacebookShare
	{
		public void Share(string url);
	}
#endif
}
