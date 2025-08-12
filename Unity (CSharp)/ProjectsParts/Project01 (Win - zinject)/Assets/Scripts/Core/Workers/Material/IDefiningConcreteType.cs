using System;
using Leguar.TotalJSON;

namespace Core.Workers.Material
{
	public interface IDefiningConcreteType
	{
		Type DefineConcreteTypeFrom(JSON json);
	}
}