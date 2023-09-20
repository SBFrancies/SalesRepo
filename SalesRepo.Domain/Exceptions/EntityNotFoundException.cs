using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRepo.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
    }

    public class EntityNotFoundException<T> : EntityNotFoundException where T : class
    {
        public int[] EntityIds { get; }

        public EntityNotFoundException(params int[] ids)
        {
            EntityIds = ids;
        }

        public override string Message => $"Entity of type {typeof(T).Name} with ID(s) {string.Join(", ", EntityIds)} could not be found";
    }
}
