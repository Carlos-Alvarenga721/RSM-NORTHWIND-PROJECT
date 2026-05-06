using System;
using System.Collections.Generic;

namespace NorthwindTraders.Infrastructure.Persistence.Entities;

public partial class CurrentProductList
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;
}
