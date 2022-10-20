using UnityEngine;
using System.Collections.Generic;
using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;

namespace HereticalSolutions.Pools.Notifiables
{
	public class NewGameObjectsPusher : IAllocationProcessor
	{
		public void Process(
			INonAllocDecoratedPool<GameObject> poolWrapper,
			IPoolElement<GameObject> currentElement,
			IPoolElement<GameObject> poppedElement = null)
		{
			if (poppedElement == null
				|| (currentElement != poppedElement))
				poolWrapper.Push(
					currentElement,
					true);
		}
	}
}