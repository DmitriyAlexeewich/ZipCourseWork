namespace ZipCourseWork.Implementation.Helpers
{
    public static class LinqHelpers
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source is null || !source.Any())
                return true;

            return false;
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> handlerFunc)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var item in source)
                handlerFunc(item);
        }
    }
}
