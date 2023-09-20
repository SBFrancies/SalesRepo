using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRepo.Domain.Exceptions
{
    public class InvalidUpdateException : Exception
    {
        public InvalidUpdateException(string message) :base(message) { }
    }

    public class InvalidUpdateException<T> : InvalidUpdateException
    {
        public InvalidUpdateException(T originalValue, T updateValue, string property) 
            : base($"Cannot update {property} from {originalValue} to {updateValue}")
        { }
    }
}
