module FsCQRSShop.Infrastructure.Railroad

type Result<'T> =
    | Success of 'T
    | Fail of string