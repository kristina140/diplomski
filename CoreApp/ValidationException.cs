using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CoreApp
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base(string.Join("\n", errors))
        {
            Errors = errors;
        }

        public ValidationException(string error)
        {
            Errors = new List<string> { error };
        }
    }

    public class ValidationPropertyException : Exception
    {
        public IDictionary<string, string> Errors { get; }
        public IEnumerable<string> ErrorsList { get; }

        public ValidationPropertyException(IDictionary<string, string> errors)
        {
            foreach (var kv in errors)
            {
                Data.Add(kv.Key, kv.Value);
            }

            Errors = errors;
            ErrorsList = errors.Values;
        }
    }
}
