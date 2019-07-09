using System;

namespace Business.Exceptions
{
    public class ResourceExistedException : Exception
    {
        public ResourceExistedException()
        {
        }

        public ResourceExistedException(string message)
            : base(message)
        {
        }
    }
}