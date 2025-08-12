using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Factorys.Base
{
	public interface IFactoryInstantiateAfter
	{
		void AfterFactoryCreate();
	}
}
