using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NXOpen.Utilities;

namespace NXOpen;

/// <summary>
/// A base class for all NXOpen tagged object collections
/// </summary>
// public abstract class TaggedObjectCollection : IEnumerable
// {
// 	internal class Enumerator : IEnumerator
// 	{
// 		private readonly byte[] state = new byte[32];

// 		private bool finished;

// 		private Tag currentTag;

// 		private readonly TaggedObjectCollection context;

// 		public object Current => NXObjectManager.Get(currentTag);

// 		internal Enumerator(TaggedObjectCollection context)
// 		{
// 			this.context = context;
// 		}

// 		public bool MoveNext()
// 		{
// 			if (finished)
// 			{
// 				return false;
// 			}
// 			int num = context.EnumerateMoveNext(ref currentTag, state);
// 			if (num != 0)
// 			{
// 				throw NXException.Create(num);
// 			}
// 			finished = currentTag == Tag.Null;
// 			return !finished;
// 		}

// 		public void Reset()
// 		{
// 			finished = false;
// 			currentTag = Tag.Null;
// 		}
// 	}

// 	public virtual void initialize()
// 	{

// 	}

// 	/// <summary>
// 	/// Returns an enumerator that iterates through a collection.
// 	/// </summary>
// 	/// <returns>An IEnumerator object that can be used to iterate through the collection. </returns>
// 	public IEnumerator GetEnumerator()
// 	{
// 		return new Enumerator(this);
// 	}

// 	/// <summary>
// 	/// Advances the enumerator to the next element of the collection. 
// 	/// </summary>
// 	/// <param name="currentTag">current tag</param>
// 	/// <param name="state">state</param>
// 	/// <returns></returns>
// 	protected abstract int EnumerateMoveNext(ref Tag currentTag, byte[] state);

// }


public abstract class TaggedObjectCollection<T> : IEnumerable<T> where T:ITaggedObject,new()
{
    internal class Enumerator<T1> : IEnumerator<T1> where T1 : ITaggedObject, new()
    {
        private readonly byte[] state = new byte[32];

        private bool finished;

        private Tag currentTag;

        private readonly TaggedObjectCollection<T1> context;


        public T1 Current => NXObjectManager.Get<T1>(currentTag);

        object IEnumerator.Current => this.Current;

        internal Enumerator(TaggedObjectCollection<T1> context)
        {
            this.context = context;
        }

        public bool MoveNext()
        {
            if (finished)
            {
                return false;
            }
            int num = context.EnumerateMoveNext(ref currentTag, state);
            if (num != 0)
            {
                throw NXException.Create(num);
            }
            finished = currentTag == Tag.Null;
            return !finished;
        }

        public void Reset()
        {
            finished = false;
            currentTag = Tag.Null;
        }

        public void Dispose()
        {
            
        }
    }

    public virtual void initialize()
    {

    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An IEnumerator object that can be used to iterate through the collection. </returns>
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }


    /// <summary>
    /// Advances the enumerator to the next element of the collection. 
    /// </summary>
    /// <param name="currentTag">current tag</param>
    /// <param name="state">state</param>
    /// <returns></returns>
    protected abstract int EnumerateMoveNext(ref Tag currentTag, byte[] state);

}

public abstract class TaggedObjectCollection2<T> : IEnumerable<T> where T : ITaggedObject
{
    internal class Enumerator2<T1> : IEnumerator<T1> where T1 : ITaggedObject
    {
        private readonly byte[] state = new byte[32];

        private bool finished;

        private Tag currentTag;

        private readonly TaggedObjectCollection2<T1> context;


        public T1 Current => NXObjectManagerEx.Get<T1>(currentTag);

        object IEnumerator.Current => this.Current;

        internal Enumerator2(TaggedObjectCollection2<T1> context)
        {
            this.context = context;
        }

        public bool MoveNext()
        {
            if (finished)
            {
                return false;
            }
            int num = context.EnumerateMoveNext(ref currentTag, state);
            if (num != 0)
            {
                throw NXException.Create(num);
            }
            finished = currentTag == Tag.Null;
            return !finished;
        }

        public void Reset()
        {
            finished = false;
            currentTag = Tag.Null;
        }

        public void Dispose()
        {
            
        }
    }

    public virtual void initialize()
    {

    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An IEnumerator object that can be used to iterate through the collection. </returns>
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator2<T>(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }


    /// <summary>
    /// Advances the enumerator to the next element of the collection. 
    /// </summary>
    /// <param name="currentTag">current tag</param>
    /// <param name="state">state</param>
    /// <returns></returns>
    protected abstract int EnumerateMoveNext(ref Tag currentTag, byte[] state);

}


