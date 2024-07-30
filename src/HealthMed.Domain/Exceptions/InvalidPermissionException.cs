using System;
using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Domain.Exceptions
{
    public class InvalidPermissionException : Exception
    {
        public Error Error { get; }

        public InvalidPermissionException(Error error) : base(error.Message)
            => Error = error;
    }
}
