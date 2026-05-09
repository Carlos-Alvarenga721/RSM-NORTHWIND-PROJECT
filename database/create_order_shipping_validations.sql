IF OBJECT_ID(N'dbo.OrderShippingValidations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderShippingValidations
    (
        OrderID int NOT NULL,
        OriginalAddress nvarchar(300) NULL,
        FormattedAddress nvarchar(300) NULL,
        Latitude float NULL,
        Longitude float NULL,
        ValidationStatus nvarchar(40) NOT NULL,
        GooglePlaceId nvarchar(200) NULL,
        ValidationMessage nvarchar(500) NULL,
        ValidationGranularity nvarchar(50) NULL,
        GeocodeGranularity nvarchar(50) NULL,
        ValidatedAtUtc datetime2 NOT NULL,
        CONSTRAINT PK_OrderShippingValidations PRIMARY KEY (OrderID),
        CONSTRAINT FK_OrderShippingValidations_Orders
            FOREIGN KEY (OrderID) REFERENCES dbo.Orders(OrderID) ON DELETE CASCADE
    );
END;
