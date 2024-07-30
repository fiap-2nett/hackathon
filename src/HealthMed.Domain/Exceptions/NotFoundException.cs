using System;
using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public Error Error { get; }

        public NotFoundException(Error error) : base(error.Message)
            => Error = error;
    }
}
