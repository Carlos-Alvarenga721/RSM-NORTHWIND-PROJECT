namespace NorthwindTraders.Application.Abstractions.Persistence;

public sealed class CreatedOrderReference(Func<int> getOrderId)
{
    public int OrderId => getOrderId();
}
