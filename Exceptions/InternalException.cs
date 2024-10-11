namespace ShareResource.Exceptions
{
    public class InternalException:Exception
    {
        public InternalException(string message, Exception inner) : base(message, inner) { }
    }
}
