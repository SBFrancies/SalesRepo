using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRepo.Domain.Interface
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
