using System;
using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public Error Error { get; }

        public DomainException(Error error) : base(error.Message)
            => Error = error;
    }
}
