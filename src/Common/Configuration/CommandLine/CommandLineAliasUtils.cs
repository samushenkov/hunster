using System.Reflection;

namespace Common.Configuration.CommandLine
{
    public static class CommandLineAliasUtils
    {
        public static IDictionary<string, string> GetMappings<TOptions>()
        {
            var attributeMappings = new Dictionary<string, string>();

            var type = typeof(TOptions);
            var typeProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in typeProperties)
            {
                var attribute = property.GetCustomAttribute<CommandLineAliasAttribute>();

                if (attribute != null)
                {
                    attributeMappings[attribute.Alias] = $"{type.Name}:{property.Name}";
                }
            }

            return attributeMappings;
        }
    }
}
