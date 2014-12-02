module FsCQRSShop.Domain.Basket

open System

open State
open Exceptions

open FsCQRSShop.Contract
open Commands
open Events

let handleBasket state pc = 
    let basketState = match state with
                        | Basket bs -> bs
                        | _ -> raise InvalidStateException
    match pc with
    | CreateBasket(id, customerId) ->
        if basketState <> initBasket then raise InvalidStateException
        [BasketCreated(id, customerId, 0)]
    | _ -> raise (NotImplementedException(""))