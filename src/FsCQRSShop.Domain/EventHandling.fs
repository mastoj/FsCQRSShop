module FsCQRSShop.Domain.EventHandling
open FsCQRSShop.Contract
open Person
open Commands

let evolve command events = 
    let initialState = match command with
                       | PersonCommand(_) -> State.Person(initPerson)
    let evolveOne state event =
        match state with
        | State.Person(p) -> evolvePerson p event
        | _ -> state
    List.fold evolveOne initialState events
