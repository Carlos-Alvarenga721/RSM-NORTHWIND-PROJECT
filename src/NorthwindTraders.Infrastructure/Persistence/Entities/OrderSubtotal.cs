using System;
using System.Collections.Generic;

namespace NorthwindTraders.Infrastructure.Persistence.Entities;

public partial class OrderSubtotal
{
    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
