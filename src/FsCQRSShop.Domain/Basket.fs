namespace FsCQRSShop.Domain
open System

open FsCQRSShop.Contract
open Commands
open Events
open Types

open Railway
open Customer
open State

module Basket =
    type Basket = 
    | Init
    | Created of BasketId

    let evolveOneBasket state event = 
        Success (Created (BasketId(Guid.Empty)))
    let evolveBasket = evolve evolveOneBasket

    let handleBasket deps pc = 
        let getState id = evolveBasket Init ((deps.readEvents id) |> (fun (_, e) -> e))

        let createBasket id customerId discountResult (version,state) = 
            match state, discountResult with
            | Init, (Success discount) -> Success (id, version, [BasketCreated(BasketId id, customerId, discount)])
            | _, Failure f -> Failure f
            | _, _ -> Failure (InvalidState "Basket")

        match pc with
        | CreateBasket(BasketId id, CustomerId customerId) ->
            let discountResult = getCustomerState deps customerId
                                 >>= (fun (v, c) -> Success c)
                                 >>= getDiscount
            getState id
            >>= createBasket id (CustomerId customerId) discountResult
        | _ -> Failure (NotSupportedCommand (pc.GetType().Name))
