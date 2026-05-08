namespace NorthwindTraders.Domain.Orders;

public static class OrderBusinessRules
{
    public const int CustomerIdMaxLength = 5;
    public const int ShipNameMaxLength = 40;
    public const int ShipAddressMaxLength = 60;
    public const int ShipCityMaxLength = 15;
    public const int ShipRegionMaxLength = 15;
    public const int ShipPostalCodeMaxLength = 10;
    public const int ShipCountryMaxLength = 15;

    public const decimal MinimumFreight = 0;
    public const decimal MinimumUnitPrice = 0;
    public const short MinimumQuantity = 1;
    public const float MinimumDiscount = 0;
    public const float MaximumDiscount = 1;
}
