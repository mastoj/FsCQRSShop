module FsCQRSShop.Domain.Customer

open System
open FsCQRSShop.Contract
open Types
open Commands
open Events

open State
open Exceptions

let handleCustomer state pc =
    let customerState = match state with
                        | Customer cs -> cs
                        | _ -> raise InvalidStateException
    match pc with
    | CreateCustomer(id, name) -> 
        if customerState <> initCustomer then raise InvalidStateException
        [CustomerCreated(id, name)]
    | MarkCustomerAsPreferred(id, discount) -> 
        if customerState = initCustomer then raise InvalidStateException
        [CustomerMarkedAsPreferred(id, discount)]

let evolveCustomer state event =
    match event with
    | CustomerCreated(id, name) -> State.Customer({Id = id; Name = name; Discount = 0} )
    | CustomerMarkedAsPreferred(id, discount) -> Customer({state with Discount = discount})
