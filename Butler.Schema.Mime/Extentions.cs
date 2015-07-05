using System.Collections.Generic;

namespace Butler.Schema.Mime {

    internal static class Extentions {

        public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> enumerator)
        {
            using (enumerator)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

    }

}