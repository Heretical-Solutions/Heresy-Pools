using System.Collections.Generic;
using HereticalSolutions.Pools.Factories;

namespace HereticalSolutions.Pools.Arguments
{
	public class ArgumentBuilder
	{
		private readonly List<IPoolDecoratorArgument> argumentChain = new List<IPoolDecoratorArgument>();

		public ArgumentBuilder Add<TArgument>(out TArgument instance) where TArgument : IPoolDecoratorArgument
		{
			instance = PoolsFactory.ActivatorAllocationDelegate<TArgument>();

			argumentChain.Add(instance);

			return this;
		}

		public IPoolDecoratorArgument[] Build()
		{
			return argumentChain.ToArray();
		}
	}
}