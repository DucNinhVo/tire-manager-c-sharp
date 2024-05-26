using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Extensions
{
    public static class IEnumExtensions
    {
        public static IEnumerable<T[]> SplitAt<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
            {
                yield return null;
            }

            if (size == 0)
            {
                yield return null;
            }

            List<T> result = new List<T>(size);
            for (int i = 0; i < size; i++)
            {
                result.Add(source.ElementAt(i));
            }
            yield return result.ToArray();
            result = new List<T>(source.Count() - size );
            for (int i = size; i < source.Count(); i++)
            {
                result.Add(source.ElementAt(i));
            }
            yield return result.ToArray();
        }

        public static IEnumerable<T[]> SplitWithInnerBrackets<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                yield return null;
            }
            List<T> result = new List<T>();
            List<int> open = new List<int>();
            for (int i = 0; i < source.Count(); i++)
            {
                if(source.ElementAt(i).ToString() == "(")
                {
                    open.Add(i);
                }
                if (source.ElementAt(i).ToString() == ")")
                    break;
            }

            for (int i = 0; i < source.Count(); i++)
            {
                if (i == open.Last())
                {
                    yield return result.ToArray();
                    result = new List<T>();
                }
                else if (i == Array.IndexOf(source.ToArray(), ")"))
                {
                    yield return result.ToArray();
                    result = new List<T>();
                }
                else
                {
                    result.Add(source.ElementAt(i));
                }
            }
            yield return result.ToArray();
          
          
           


        }
    }
}
