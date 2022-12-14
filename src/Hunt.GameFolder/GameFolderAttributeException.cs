
namespace Hunt.GameFolder
{
    public class GameFolderException : Exception
    {
        public GameFolderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GameFolderException(string message)
            : base(message)
        {
        }

        protected GameFolderException()
        {
        }
    }

    public class GameFolderAttributeException : GameFolderException
    {
        public GameFolderAttributeException(string attributeName)
        {
            AttributeName = attributeName;
        }

        public string AttributeName { get; private set; }
    }

    public class GameFolderAttributeValueException<T> : GameFolderAttributeException
    {
        public GameFolderAttributeValueException(string attributeName, T attributeValue)
            : base(attributeName)
        {
            AttributeValue = attributeValue;
        }

        public T AttributeValue { get; private set; }
    }
}