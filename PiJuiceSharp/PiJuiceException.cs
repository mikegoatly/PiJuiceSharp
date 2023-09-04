namespace PiJuiceSharp
{
    public class PiJuiceException : Exception
    {
        public PiJuiceException(string message) : base(message)
        {
        }

        public PiJuiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PiJuiceException()
        {
        }
    }
}
