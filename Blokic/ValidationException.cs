using System;
using System.Collections;

namespace Blokic
{
    public class ValidationException : Exception
    {
        public IEnumerable Errors { get; }

        public ValidationException(IEnumerable errors)
            : base(string.Join("\n", errors))
        {
            Errors = errors;
        }
    }
}
