using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator
{
    public static class ExtensionMethods
    {
        public static bool AllEqual<T, U>(this IEnumerable<T> enumerable, Func<T, U> map)
            where U : IEquatable<U>
        {
            U first = map(enumerable.First());
            return enumerable.All(t => map(t).Equals(first));
        }

        public static bool AllEqual<T>(this IEnumerable<T> enumerable)
            where T : IEquatable<T>
        {
            return enumerable.AllEqual(t => t);
        }
    }

    public struct Counter<T>
    {
        public T Item
        {
            get;
            private set;
        }

        public int Count
        {
            get;
            private set;
        }

        public Counter(T item, int count)
            : this()
        {
            Item = item;
            Count = count;
        }
    }
}
