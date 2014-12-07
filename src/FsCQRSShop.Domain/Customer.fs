module FsCQRSShop.Domain.Customer

open System
open FsCQRSShop.Contract
open Types
open Commands
open Events

open State

open FsCQRSShop.Infrastructure.Railroad

let evolveOneCustomer state event =
    match event with
    | CustomerCreated(id, name) -> {Id = id; Name = name; Discount = 0}
    | CustomerMarkedAsPreferred(id, discount) -> {state with Discount = discount}

let evolveCustomer = evolve evolveOneCustomer

let getCustomerState deps id = evolveCustomer initCustomer ((deps.readEvents id) |> (fun (_, e) -> e))

let handleCustomer deps pc =
    match pc with
    | CreateCustomer(CustomerId id, name) -> 
        let (version, state) = getCustomerState deps id
        if state <> initCustomer then Fail (InvalidState "Customer")
        else Success (id, version, [CustomerCreated(CustomerId id, name)])
    | MarkCustomerAsPreferred(CustomerId id, discount) -> 
        let (version, state) = getCustomerState deps id
        if state = initCustomer then Fail (InvalidState "Customer")
        else Success (id, version, [CustomerMarkedAsPreferred(CustomerId id, discount)])

