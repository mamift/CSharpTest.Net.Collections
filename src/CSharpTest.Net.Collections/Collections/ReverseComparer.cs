using System.Collections.Generic;

namespace CSharpTest.Net.Collections
{
    /// <summary>
    /// Compare things in reverse order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ReverseComparer<T> Default = new ReverseComparer<T>(Comparer<T>.Default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static ReverseComparer<T> Reverse(IComparer<T> comparer)
        {
            return new ReverseComparer<T>(comparer);
        }

        private readonly IComparer<T> comparer = Default;

        private ReverseComparer(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            return comparer.Compare(y, x);
        }
    }
}