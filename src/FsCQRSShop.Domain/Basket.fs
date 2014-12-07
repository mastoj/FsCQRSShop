module FsCQRSShop.Domain.Basket

open System

open State
open Exceptions

open FsCQRSShop.Contract
open Commands
open Events
open Types

open Customer

let evolveOneBasket a b = {Id = BasketId(Guid.Empty)}
let evolveBasket = evolve evolveOneBasket

let handleBasket deps pc = 
    let getState (BasketId id) = evolveBasket initBasket ((deps.readEvents id) |> (fun (_, e) -> e))
    match pc with
    | CreateBasket(id, customerId) ->
        let customerState = getCustomerState deps customerId
        if customerState = initCustomer then raise InvalidStateException
        let state = getState id
        if state <> initBasket then raise InvalidStateException
        [BasketCreated(id, customerId, customerState.Discount)]
    | _ -> raise (NotImplementedException(""))