module FsCQRSShop.Domain.EventHandling
open FsCQRSShop.Contract
open State
open Customer
open Commands

let evolve command events = 
    let initialState = match command with
                       | CustomerCommand(_) -> State.Customer(initCustomer)
    let evolveOne state event =
        match state with
        | State.Customer p -> evolveCustomer p event
        | _ -> state
    List.fold evolveOne initialState events
