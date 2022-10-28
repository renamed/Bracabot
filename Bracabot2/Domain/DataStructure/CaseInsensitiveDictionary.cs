namespace Bracabot2.Domain.DataStructure
{
    public class CaseInsensitiveDictionary<V> : Dictionary<string, V>
    {
        public CaseInsensitiveDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(IDictionary<string, V> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(IEnumerable<KeyValuePair<string, V>> collection) : base(collection, StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(IEqualityComparer<string> comparer) : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(IDictionary<string, V> dictionary, IEqualityComparer<string> comparer) : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(IEnumerable<KeyValuePair<string, V>> collection, IEqualityComparer<string> comparer) : base(collection, StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
