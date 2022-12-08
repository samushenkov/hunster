namespace Common.Configuration.CommandLine
{
    public class CommandLineAliasAttribute : Attribute
    {
        public string Alias { get; }

        public CommandLineAliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}
