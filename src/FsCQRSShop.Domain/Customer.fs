namespace FsCQRSShop.Domain
open System
open FsCQRSShop.Contract
open Types
open Commands
open Events

open Helpers
open Railway

module Customer =
    type CustomerInfo = {Id:CustomerId; Name:string} 

    type Customer = 
        | Init
        | Created of CustomerInfo
        | Preferred of CustomerInfo * Discount:int

    let invalidCustomerState = Failure (InvalidState "Customer")
    
    let getDiscount = function
        | Preferred (_, d) -> Success d
        | Created _ -> Success 0
        | Init -> invalidCustomerState

    let getCustomerId = function
        | Preferred (info, _) -> Success info.Id
        | Created info -> Success info.Id
        | Init -> invalidCustomerState

    let evolveOneCustomer state event =
        match state with
        | Init -> match event with
                  | CustomerCreated(id, name) -> Success ( Created{Id = id; Name = name})
                  | _ -> stateTransitionFail event state
        | Created info -> match event with
                          | CustomerMarkedAsPreferred(id, discount) -> Success (Preferred(info,discount))
                          | _ -> stateTransitionFail event state
        | Preferred (info, _) -> match event with
                                 | CustomerMarkedAsPreferred(id, discount) -> Success (Preferred(info,discount))
                                 | _ -> stateTransitionFail event state

    let evolveCustomer = evolve evolveOneCustomer

    let getCustomerState deps id = evolveCustomer Init ((deps.readEvents id) |> (fun (_, e) -> e))

    let handleCustomer deps cc =
        let createCustomer id name (version, state) =
            match state with
            | Init -> Success (id, version, [CustomerCreated(CustomerId id, name)])
            | _ -> Failure (InvalidState "Customer")
        let markAsPreferred id discount (version, state) = 
            match state with
            | Init -> Failure (InvalidState "Customer")
            | _ -> Success (id, version, [CustomerMarkedAsPreferred(CustomerId id, discount)])

        match cc with
        | CreateCustomer(CustomerId id, name) -> 
            getCustomerState deps id >>= (createCustomer id name)
        | MarkCustomerAsPreferred(CustomerId id, discount) -> 
            getCustomerState deps id >>= (markAsPreferred id discount)

