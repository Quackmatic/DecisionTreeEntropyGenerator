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

        public static U GetOrDefault<T, U>(this T t, Func<T, U> map, U defaultValue)
        {
            if(t == null)
            {
                return defaultValue;
            }
            else
            {
                return map(t);
            }
        }

        public static string Repeat(this string s, int repetitions)
        {
            StringBuilder sb = new StringBuilder(s.Length * repetitions);
            for(int i = 0; i < repetitions; i++)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static string Indent(this string s, int levels, string indentation = " ")
        {
            StringBuilder sb = new StringBuilder(s.Length);
            int newLineLength = Environment.NewLine.Length;
            for (int i = 0; i < s.Length; i++)
            {
                if(i <= s.Length - newLineLength &&
                    s.Substring(i, newLineLength) == Environment.NewLine)
                {
                    i += newLineLength - 1;
                    sb.Append(Environment.NewLine);
                    for(int j = 0; j < levels; j++)
                    {
                        sb.Append(indentation);
                    }
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
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
