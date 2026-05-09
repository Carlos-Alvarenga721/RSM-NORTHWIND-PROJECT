namespace NorthwindTraders.Infrastructure.Persistence.Entities;

public partial class OrderShippingValidation
{
    public int OrderId { get; set; }

    public string? OriginalAddress { get; set; }

    public string? FormattedAddress { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string ValidationStatus { get; set; } = null!;

    public string? GooglePlaceId { get; set; }

    public string? ValidationMessage { get; set; }

    public string? ValidationGranularity { get; set; }

    public string? GeocodeGranularity { get; set; }

    public DateTime ValidatedAtUtc { get; set; }

    public virtual Order Order { get; set; } = null!;
}
