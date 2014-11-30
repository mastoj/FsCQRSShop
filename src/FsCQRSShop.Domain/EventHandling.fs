module FsCQRSShop.Domain.EventHandling
open FsCQRSShop.Contract
open State
open Commands

open Customer
open Product

let evolve command events = 
    let initialState = match command with
                       | CustomerCommand(_) -> State.Customer(initCustomer)
                       | ProductCommand(_) -> State.Product(initProduct)
    let evolveOne state event =
        match state with
        | State.Customer c -> evolveCustomer c event
        | State.Product p -> evolveProduct p event
        | _ -> state
    List.fold evolveOne initialState events
