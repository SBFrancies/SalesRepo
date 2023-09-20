using SalesRepo.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRepo.Domain.Helpers
{
    public static class OrderStatusUpdates
    {
        public static IReadOnlyDictionary<OrderStatus, IEnumerable<OrderStatus>> ValidUpdates
            => new Dictionary<OrderStatus, IEnumerable<OrderStatus>>
            {
                [OrderStatus.Pending] = new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled },
                [OrderStatus.Shipped] = new List<OrderStatus> { OrderStatus.Delivered },
                [OrderStatus.Delivered] = new List<OrderStatus> { OrderStatus.Returned },
                [OrderStatus.Cancelled] = new List<OrderStatus>(),
                [OrderStatus.Returned] = new List<OrderStatus>(),
            };
    }
}
