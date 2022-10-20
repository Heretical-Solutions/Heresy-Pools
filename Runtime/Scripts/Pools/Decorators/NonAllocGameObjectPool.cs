using UnityEngine;
using HereticalSolutions.Collections;
using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Notifiables;

namespace HereticalSolutions.Pools
{
	public class NonAllocGameObjectPool : ANonAllocDecoratorPool<GameObject>
	{
		private Transform poolParentTransform;

		private CompositeGameObjectNotifiable gameObjectNotifiable;

		public NonAllocGameObjectPool(INonAllocDecoratedPool<GameObject> innerPool,
			Transform parentTransform,
			CompositeGameObjectNotifiable gameObjectNotifiable = null)
			: base(innerPool)
		{
			this.poolParentTransform = parentTransform;

			this.gameObjectNotifiable = gameObjectNotifiable;
		}

		protected override void OnAfterPop(
			IPoolElement<GameObject> instance,
			IPoolDecoratorArgument[] args)
		{
			var value = instance.Value;

			if (value == null)
				return;

			Transform newParentTransform = null;

			bool worldPositionStays = true;

			if (args.TryGetArgument<ParentTransformArgument>(out var arg1))
			{
				newParentTransform = arg1.Parent;

				worldPositionStays = arg1.WorldPositionStays;
			}

			value.transform.SetParent(newParentTransform, worldPositionStays);

			if (args.TryGetArgument<WorldPositionArgument>(out var arg2))
			{
				value.transform.position = arg2.Position;
			}

			if (args.TryGetArgument<WorldRotationArgument>(out var arg3))
			{
				value.transform.rotation = arg3.Rotation;
			}

			if (args.TryGetArgument<LocalPositionArgument>(out var arg4))
			{
				value.transform.localPosition = arg2.Position;
			}

			if (args.TryGetArgument<LocalRotationArgument>(out var arg5))
			{
				value.transform.localRotation = arg3.Rotation;
			}

			value.SetActive(true);

			gameObjectNotifiable?.Process(instance);
		}

		protected override void OnBeforePush(IPoolElement<GameObject> instance)
		{
			var value = instance.Value;

			value.SetActive(false);

			value.transform.SetParent(poolParentTransform);

			value.transform.localPosition = Vector3.zero;

			value.transform.localRotation = Quaternion.identity;
		}

		protected override void OnAfterPush(IPoolElement<GameObject> instance)
		{
			gameObjectNotifiable?.Process();
		}
	}
}