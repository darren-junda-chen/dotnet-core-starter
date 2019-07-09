using System;

namespace Business.Exceptions
{
    public class PaymentRequiredException : Exception
    {
        public PaymentRequiredException()
        {
        }

        public PaymentRequiredException(string message)
            : base(message)
        {
        }
    }
}