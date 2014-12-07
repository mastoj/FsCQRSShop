module FsCQRSShop.Domain.Customer

open System
open FsCQRSShop.Contract
open Types
open Commands
open Events

open State
open Exceptions

//let evolve initState events =
//    let evolveOne state event =
//        match state with
//        | State.Customer c -> evolveCustomer c event
//        | State.Product p -> evolveProduct p event
//        | _ -> state
//    List.fold evolveOne initState events

let evolveOneCustomer state event =
    match event with
    | CustomerCreated(id, name) -> {Id = id; Name = name; Discount = 0}
    | CustomerMarkedAsPreferred(id, discount) -> {state with Discount = discount}

let evolveCustomer = evolve evolveOneCustomer

let getCustomerState deps (CustomerId id) = evolveCustomer initCustomer ((deps.readEvents id) |> (fun (_, e) -> e))

let handleCustomer deps pc =
    match pc with
    | CreateCustomer(id, name) -> 
        let state = getCustomerState deps id
        if state <> initCustomer then raise InvalidStateException
        [CustomerCreated(id, name)]
    | MarkCustomerAsPreferred(id, discount) -> 
        let state = getCustomerState deps id
        if state = initCustomer then raise InvalidStateException
        [CustomerMarkedAsPreferred(id, discount)]

