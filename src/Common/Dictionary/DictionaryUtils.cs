
namespace Common.Dictionary
{
    public static class DictionaryUtils
    {
        public static IDictionary<TKey, TValue> Union<TKey, TValue>(params IDictionary<TKey, TValue>[] dictionaryList)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var dictionary in dictionaryList)
            {
                foreach (var (key, value) in dictionary)
                {
                    result[key] = value;
                }
            }

            return result;
        }
    }
}
