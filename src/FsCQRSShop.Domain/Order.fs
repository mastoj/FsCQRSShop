namespace FsCQRSShop.Domain
open System
open Railway

module Order =
    let handleOrder state pc = Failure (NotSupportedCommand (pc.GetType().Name))