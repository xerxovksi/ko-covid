namespace KO.Covid.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

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

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) =>
            collection == null || collection.Any() == false;

        public static HashSet<T> AddRange<T>(this HashSet<T> originalSet, HashSet<T> newSet)
        {
            if (newSet.IsNullOrEmpty())
            {
                return originalSet;
            }

            foreach (var newItem in newSet)
            {
                if (originalSet.Contains(newItem))
                {
                    continue;
                }

                originalSet.Add(newItem);
            }

            return originalSet;
        }

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

        public static string ToJson<T>(this T value) =>
            value == null
            ? default
            : JsonConvert.SerializeObject(value);

        public static T FromJson<T>(this string value) =>
            string.IsNullOrEmpty(value)
            ? default
            : JsonConvert.DeserializeObject<T>(value);

        public static string ToSHA256(this string value)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

        public static List<T> DistinctBy<T>(
            this List<T> collection,
            Func<T, string> getKey,
            Func<T, T, bool> comparator)
        {
            if (collection.IsNullOrEmpty() || collection.Count.Equals(1))
            {
                return collection;
            }

            var distinctCollection = new Dictionary<string, T>
            {
                { getKey(collection.First()), collection.First() }
            };

            for (var i = 1; i < collection.Count; i++)
            {
                var key = getKey(collection[i]);
                if (distinctCollection.ContainsKey(key))
                {
                    if (comparator(collection[i], distinctCollection[key]))
                    {
                        distinctCollection[key] = collection[i];
                    }

                    continue;
                }

                distinctCollection[key] = collection[i];
            }

            return distinctCollection.Values.ToList();
        }

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

        public static List<List<T>> GetBatches<T>(this List<T> collection, int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new InvalidOperationException("Batch size cannot be null.");
            }

            var itemsInBatches = new List<List<T>>();

            for (var i = 0; i < collection.Count; i++)
            {
                if (itemsInBatches.Count.Equals(i / batchSize))
                {
                    itemsInBatches.Add(new List<T>());
                }

                itemsInBatches[i / batchSize].Add(collection[i]);
            }

            return itemsInBatches;
        }
    }
}
