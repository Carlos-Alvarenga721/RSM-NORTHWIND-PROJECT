using System;
using System.Collections.Generic;

namespace NorthwindTraders.Infrastructure.Persistence.Entities;

public partial class CategorySalesFor1997
{
    public string CategoryName { get; set; } = null!;

    public decimal? CategorySales { get; set; }
}
