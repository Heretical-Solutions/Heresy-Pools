using System;
using UnityEngine;
using Zenject;

using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.Pools.Factories
{
	public static class PoolFactory
	{
		#region Non alloc pool

		public static INonAllocPool<GameObject> BuildGameObjectPool(
			BuildNonAllocGameObjectPoolCommand command)
		{
			Func<GameObject> valueAllocationDelegate = (command.Container != null)
				? () => { return command.Container.InstantiatePrefab(command.Prefab); }
				: () => { return GameObject.Instantiate(command.Prefab); };

			if (command.CollectionType == typeof(PackedArrayPool<GameObject>))
				return CollectionFactory.BuildPackedArrayPool<GameObject>(
					valueAllocationDelegate,
					command.ContainerAllocationDelegate,
					command.InitialAllocation,
					command.AdditionalAllocation);

			if (command.CollectionType == typeof(SupplyAndMergePool<GameObject>))
				return CollectionFactory.BuildSupplyAndMergePool<GameObject>(
					valueAllocationDelegate,
					command.ContainerAllocationDelegate,
					command.InitialAllocation,
					command.AdditionalAllocation);

			throw new Exception($"[PoolFactory] INVALID COLLECTION TYPE: {{ {command.CollectionType.ToString()} }}");
		}

		#endregion

		#region Pool elements

		public static PoolElementWithVariant<T> BuildPoolElementWithVariant<T>(
			Func<T> allocationDelegate)
		{
			PoolElementWithVariant<T> result = new PoolElementWithVariant<T>(allocationDelegate());

			return result;
		}

		public static IPoolElement<T> BuildValueAssignedNotifyingPoolElement<T>(
			Func<T> allocationDelegate,
			IValueAssignedNotifiable<T> notifiable)
		{
			ValueAssignedNotifyingPoolElement<T> result = new ValueAssignedNotifyingPoolElement<T>(
				allocationDelegate(),
				notifiable);

			return result;
		}

		public static Func<Func<T>, IPoolElement<T>> BuildValueAssignedNotifyingPoolElementAllocationDelegate<T>(
			IValueAssignedNotifiable<T> notifiable)
		{
			return (valueAllocationDelegate) =>
			{
				return BuildValueAssignedNotifyingPoolElement(
					valueAllocationDelegate,
					notifiable);
			};
		}

		#endregion
	}
}