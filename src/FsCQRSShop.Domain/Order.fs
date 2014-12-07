module FsCQRSShop.Domain.Order
open System
open FsCQRSShop.Infrastructure.Railroad

let handleOrder state pc = Fail (NotSupportedCommand (pc.GetType().Name))