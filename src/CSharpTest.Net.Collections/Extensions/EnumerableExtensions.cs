using System;
using System.Collections;
using System.Collections.Generic;

namespace CSharpTest.Net.Extensions
{
#pragma warning disable CS1591
    public static class EnumerableExtensions
#pragma warning restore CS1591
    {
        /// <summary>
        /// Attempts to call MoveNext on the enumerator, returning null if an exception occurs, otherwise the regular boolean values of true or false.
        /// <para>It does not return false on an exception as that will be indistinguishable from a normal end of the enumeration.</para>
        /// <para>When this method returns null, callers should continue to call <see cref="IEnumerator{T}.MoveNext"/>, until an actual <c>false</c> value is returned.</para>
        /// <para>In words check for <c>while(items.TryMoveNext() == null || items.TryMoveNext() == true)</c> or for
        /// <c>while(items.TryMoveNext() != false)</c>.</para>
        /// </summary>
        /// <param name="enumerator"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool? TryMoveNext<T>(this IEnumerator<T> enumerator)
        {
            try
            {
                return enumerator.MoveNext();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Allows catching exceptions that occur during EACH iteration of the enumerator. When an exception occurs, the default value for the type is returned.
        /// <para>In other words, individual iterations where exceptions are thrown ARE NOT skipped. This ensures that LINQ methods like Count() return expected values.</para>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="eachIteration">Optional; executes a function with each successful (non-exception throwing) iteration.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns either the current element in the enumerator or <c>default(T)</c></returns>
        public static IEnumerable<T> TryEnumerate<T>(this IEnumerable<T> enumerable, Func<T, T> eachIteration = null)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                var didMove = enumerator.TryMoveNext();
                while (didMove == true || didMove == null)
                {
                    if (didMove is null) yield return default;
                    var current = eachIteration != null ? eachIteration(enumerator.Current) : enumerator.Current;
                    yield return current;

                    didMove = enumerator.TryMoveNext();
                }
            }
        }
    }
}