module FsCQRSShop.Infrastructure.Railroad

type Error = 
    | InvalidState of string
    | NotSupportedCommand of string

type Result<'T> =
    | Success of 'T
    | Fail of Error
