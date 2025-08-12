using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Factorys.Base
{
/// <summary>
/// Вызывается после создания обьекта пулом
/// </summary>
	public interface IInitAfterCreatePool
	{
		void InitAfterCreatePool();
	}
}
