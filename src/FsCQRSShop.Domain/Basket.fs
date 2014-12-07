module FsCQRSShop.Domain.Basket

open System

open State

open FsCQRSShop.Contract
open Commands
open Events
open Types

open FsCQRSShop.Infrastructure.Railroad

open Customer

let evolveOneBasket a b = {Id = BasketId(Guid.Empty)}
let evolveBasket = evolve evolveOneBasket

let handleBasket deps pc = 
    let getState id = evolveBasket initBasket ((deps.readEvents id) |> (fun (_, e) -> e))
    match pc with
    | CreateBasket(BasketId id, CustomerId customerId) ->
        let (_, customerState) = getCustomerState deps customerId
        if customerState = initCustomer then Fail (InvalidState "Customer")
        else
            let (version, state) = getState id
            if state <> initBasket then Fail (InvalidState "Basket")
            else Success (id, version, [BasketCreated(BasketId id, CustomerId customerId, customerState.Discount)])
    | _ -> Fail (NotSupportedCommand (pc.GetType().Name))