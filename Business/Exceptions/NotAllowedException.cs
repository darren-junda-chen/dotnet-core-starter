using System;

namespace Business.Exceptions
{
    public class NotAllowedException : Exception
    {
        public NotAllowedException()
        {
        }

        public NotAllowedException(string message)
            : base(message)
        {
        }
    }
}