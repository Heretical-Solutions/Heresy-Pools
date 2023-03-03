using System;
using HereticalSolutions.Collections;

namespace HereticalSolutions.Pools.GenericNonAlloc
{
    /// <summary>
    /// The container that combines the functions of a memory pool and a list with an increased performance
    /// Basic concept is:
    /// 1. The contents are pre-allocated
    /// 2. Popping a new item is actually retrieving the first unused item and increasing the last used item index
    /// 3. Pushing an item is taking the last used item, swapping it with the removed item and decreasing the last used item index
    /// </summary>
    /// <typeparam name="T">Type of the objects stored in the container</typeparam>
    public class PackedArrayPool<T>
        : IFixedSizeCollection<IPoolElement<T>>,
	      INonAllocPool<T>,
          IIndexable<IPoolElement<T>>,
          IModifiable<IPoolElement<T>[]>
    {
        private IPoolElement<T>[] contents;
        
        private int count;

        public PackedArrayPool(IPoolElement<T>[] contents)
        {
            this.contents = contents;
            
            count = 0;
        }
        
        #region IFixedSizeCollection

        public int Capacity { get { return contents.Length; } }
        
        public IPoolElement<T> ElementAt(int index)
        {
	        return contents[index];
        }

        #endregion

		#region IModifiable

		public IPoolElement<T>[] Contents { get { return contents; } }
		
		public void UpdateContents(IPoolElement<T>[] newContents)
        {
            contents = newContents;
        }
		
		public void UpdateCount(int newCount)
		{
			count = newCount;
		}

		#endregion

		#region IIndexable

		public int Count { get { return count; } }
		
		public IPoolElement<T> this[int index]
		{
			get
			{
                if (index >= count || index < 0)
					throw new Exception(
                        string.Format(
							"[IndexedPackedArray<{0}>] INVALID INDEX: {1} COUNT:{2} CAPACITY:{3}",
                            typeof(T).ToString(),
                            index,
                            Count,
                            Capacity));

				return contents[index];
			}
		}
		
		public IPoolElement<T> Get(int index)
		{
			if (index >= count || index < 0)
				throw new Exception(
					string.Format(
						"[IndexedPackedArray<{0}>] INVALID INDEX: {1} COUNT:{2} CAPACITY:{3}",
						typeof(T).ToString(),
						index,
						Count,
						Capacity));

			return contents[index];
		}

        #endregion

        #region INonAllocPool

		public IPoolElement<T> Pop()
        {
            var result = contents[count];

            result.Metadata.Get<IIndexed>().Index = count;
            
            count++;

            return result;
        }

		public IPoolElement<T> Pop(int index)
		{
            if (index < count)
            {
                throw new Exception($"[IndexedPackedArray] ELEMENT AT INDEX {index} IS ALREADY POPPED");
			}


			int lastFreeItemIndex = count;

			if (index != lastFreeItemIndex)
			{
				IIndexed lastFreeItemAsIndexed = contents[lastFreeItemIndex].Metadata.Get<IIndexed>();
				
				IIndexed itemAtIndexAsIndexed = contents[index].Metadata.Get<IIndexed>();
				
				
				lastFreeItemAsIndexed.Index = -1;

				itemAtIndexAsIndexed.Index = index;


				var swap = contents[index];

				contents[index] = contents[lastFreeItemIndex];

				contents[lastFreeItemIndex] = swap;
			}
			else
			{
				contents[index].Metadata.Get<IIndexed>().Index = index;
			}


			var result = contents[lastFreeItemIndex];

			count++;

			return result;
		}

        public void Push(IPoolElement<T> item)
        {
            Push(item.Metadata.Get<IIndexed>().Index);
        }

        public void Push(int index)
        {
            if (index >= count)
            {
	            return;
            }

            int lastItemIndex = count - 1;

            if (index != lastItemIndex)
            {
	            IIndexed lastItemAsIndexed = contents[lastItemIndex].Metadata.Get<IIndexed>();
				
	            IIndexed itemAtIndexAsIndexed = contents[index].Metadata.Get<IIndexed>();
	            
	            
	            lastItemAsIndexed.Index = index;

	            itemAtIndexAsIndexed.Index = -1;


                var swap = contents[index];

                contents[index] = contents[lastItemIndex];

                contents[lastItemIndex] = swap;
            }
            else
            {
				contents[index].Metadata.Get<IIndexed>().Index = -1;
            }

            count--;
        }

        public bool HasFreeSpace { get { return count < contents.Length; } }
        
        #endregion
    }
}