using Game.Scripts.Controllers.AccuracyLabels.Items;

namespace Game.Scripts.Controllers.AccuracyLabels.Factorys
{
	public interface IAccuracyLabelsFactory
	{
		AccuracyLabel GetInstance(string accuracy);
	}
}