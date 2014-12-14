namespace FsCQRSShop.Domain
open FsCQRSShop.Contract
open Types
open System
open Events
open Railway

module Helpers =
    let evolve evolveOne initState events =
        List.fold (fun result e -> match result with
                                   | Failure f -> Failure f
                                   | Success (v,s) -> match (evolveOne s e) with
                                                      | Success s -> Success (v+1, s) 
                                                      | Failure f -> Failure f) 
                  (Success (-1, initState)) events

    let getTypeName o = o.GetType().Name
    let stateTransitionFail event state = Failure (InvalidStateTransition (sprintf "Invalid event %s for state %s" (event |> getTypeName) (state |> getTypeName)))
