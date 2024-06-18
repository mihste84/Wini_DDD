namespace DatabaseDapper.Queries;

public static class BookingRowQueries
{
    public const string DeleteMultiple = """
        DELETE FROM dbo.BookingRows
        WHERE BookingId = @BookingId AND RowNumber IN @Rows
    """;

    public const string UpdateIsDeleted = """
        UPDATE dbo.BookingRows
            SET IsDeleted = @IsDeleted
        WHERE BookingId = @BookingId AND RowNumber IN @Rows
    """;

    public const string UpdateAuthorizeByRow = """
        UPDATE dbo.BookingRows
            SET IsAuthorized = 1
        WHERE BookingId = @BookingId AND RowNumber IN @Rows
    """;

    public const string UpdateRemoveAllAuthorizations = """
        UPDATE dbo.BookingRows
            SET IsAuthorized = 0
        WHERE BookingId = @BookingId
    """;

    public const string Upsert = """
        UPDATE dbo.BookingRows
        SET BookingId = @BookingId,
            RowNumber = @RowNumber,
            BusinessUnit = @BusinessUnit,
            Account = @Account,
            Subsidiary = @Subsidiary,
            Subledger = @Subledger,
            SubledgerType = @SubledgerType,
            CostObject1 = @CostObject1,
            CostObjectType1 = @CostObjectType1,
            CostObject2 = @CostObject2,
            CostObjectType2 = @CostObjectType2,
            CostObject3 = @CostObject3,
            CostObjectType3 = @CostObjectType3,
            CostObject4 = @CostObject4,
            CostObjectType4 = @CostObjectType4,
            Remark = @Remark,
            Authorizer= @Authorizer,
            IsAuthorized = @IsAuthorized,
            Amount = @Amount,
            CurrencyCode = @CurrencyCode,
            ExchangeRate = @ExchangeRate
        WHERE BookingId = @BookingId AND RowNumber = @RowNumber

        IF (@@ROWCOUNT = 0)
        BEGIN      
                INSERT INTO dbo.BookingRows (
                BookingId,
                RowNumber,
                BusinessUnit,
                Account,
                Subsidiary,
                Subledger,
                SubledgerType,
                CostObject1,
                CostObjectType1,
                CostObject2,
                CostObjectType2,
                CostObject3,
                CostObjectType3,
                CostObject4,
                CostObjectType4,
                Remark,
                Authorizer,
                IsAuthorized,
                Amount,
                CurrencyCode,
                ExchangeRate
            )
            VALUES(
                @BookingId,
                @RowNumber,
                @BusinessUnit,
                @Account,
                @Subsidiary,
                @Subledger,
                @SubledgerType,
                @CostObject1,
                @CostObjectType1,
                @CostObject2,
                @CostObjectType2,
                @CostObject3,
                @CostObjectType3,
                @CostObject4,
                @CostObjectType4,
                @Remark,
                @Authorizer,
                @IsAuthorized,
                @Amount,
                @CurrencyCode,
                @ExchangeRate
            )
        END 
    """;

    public const string Update = """
        UPDATE dbo.BookingRows
        SET BookingId = @BookingId,
            RowNumber = @RowNumber,
            BusinessUnit = @BusinessUnit,
            Account = @Account,
            Subsidiary = @Subsidiary,
            Subledger = @Subledger,
            SubledgerType = @SubledgerType,
            CostObject1 = @CostObject1,
            CostObjectType1 = @CostObjectType1,
            CostObject2 = @CostObject2,
            CostObjectType2 = @CostObjectType2,
            CostObject3 = @CostObject3,
            CostObjectType3 = @CostObjectType3,
            CostObject4 = @CostObject4,
            CostObjectType4 = @CostObjectType4,
            Remark = @Remark,
            Authorizer= @Authorizer,
            IsAuthorized = @IsAuthorized,
            Amount = @Amount,
            CurrencyCode = @CurrencyCode,
            ExchangeRate = @ExchangeRate
        WHERE BookingId = @BookingId AND RowNumber = @RowNumber
    """;

    public const string Insert = """
        INSERT INTO dbo.BookingRows (
            BookingId,
            RowNumber,
            BusinessUnit,
            Account,
            Subsidiary,
            Subledger,
            SubledgerType,
            CostObject1,
            CostObjectType1,
            CostObject2,
            CostObjectType2,
            CostObject3,
            CostObjectType3,
            CostObject4,
            CostObjectType4,
            Remark,
            Authorizer,
            IsAuthorized,
            Amount,
            CurrencyCode,
            ExchangeRate
        )
        VALUES(
            @BookingId,
            @RowNumber,
            @BusinessUnit,
            @Account,
            @Subsidiary,
            @Subledger,
            @SubledgerType,
            @CostObject1,
            @CostObjectType1,
            @CostObject2,
            @CostObjectType2,
            @CostObject3,
            @CostObjectType3,
            @CostObject4,
            @CostObjectType4,
            @Remark,
            @Authorizer,
            @IsAuthorized,
            @Amount,
            @CurrencyCode,
            @ExchangeRate
        )
    """;
}