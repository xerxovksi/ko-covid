namespace KO.Covid.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DomainExtensions
    {
        public static T DeepCopy<T>(this T source) =>
            JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(source));

        public static T GetValue<T>(this Dictionary<string, T> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out T value))
            {
                return value;
            }

            return default;
        }

        public static bool IsNullOrEmpty<T>(this List<T> collection) =>
            collection == null || collection.Any() == false;

        public static bool EqualsIgnoreCase(this string value, string compareWith) =>
            value?.Equals(compareWith, StringComparison.InvariantCultureIgnoreCase) ?? false;

        public static T CheckNull<T>(this T item) where T : class
        {
            var isNotNull = item.GetType().GetProperties()
                .Select(property => property.GetValue(item))
                .Any(value => value != null);

            return isNotNull ? item : null;
        }

        public static string WithId(this string value, object id) => $"[{id}] {value}";

        public static List<string> ToValuesList(
            this string value,
            string recordDelimiter,
            string expressionDelimiter) =>
            value.ToDictionary(recordDelimiter, expressionDelimiter)?.Values?.ToList()
            ?? new List<string>();

        public static Dictionary<string, string> ToDictionary(
            this string value,
            string recordDelimiter,
            string expressionDelimiter)
        {
            if (string.IsNullOrEmpty(value)
                || string.IsNullOrEmpty(recordDelimiter)
                || string.IsNullOrEmpty(expressionDelimiter))
            {
                return new Dictionary<string, string>();
            }

            var records = value.Split(recordDelimiter);
            var results = new Dictionary<string, string>();
            foreach (var record in records)
            {
                var expression = record.Split(expressionDelimiter);
                results.Add(expression.FirstOrDefault(), expression.LastOrDefault());
            }

            return results;
        }
    }
}
